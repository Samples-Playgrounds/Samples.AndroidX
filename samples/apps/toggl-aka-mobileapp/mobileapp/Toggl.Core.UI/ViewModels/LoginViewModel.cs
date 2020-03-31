using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.Exceptions;
using Toggl.Core.Extensions;
using Toggl.Core.Interactors;
using Toggl.Core.Login;
using Toggl.Core.Services;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
using Toggl.Networking.Exceptions;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage.Settings;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class LoginViewModel : ViewModelWithInput<CredentialsParameter>
    {
        [Flags]
        public enum ShakeTargets
        {
            None = 0,
            Email = 1,
            Password = 2
        }

        private readonly IUserAccessManager userAccessManager;
        private readonly IAnalyticsService analyticsService;
        private readonly IOnboardingStorage onboardingStorage;
        private readonly IErrorHandlingService errorHandlingService;
        private readonly ILastTimeUsageStorage lastTimeUsageStorage;
        private readonly ITimeService timeService;
        private readonly ISchedulerProvider schedulerProvider;
        private readonly IRxActionFactory rxActionFactory;
        private readonly IInteractorFactory interactorFactory;

        private IDisposable loginDisposable;

        private readonly Subject<ShakeTargets> shakeSubject = new Subject<ShakeTargets>();
        private readonly Subject<bool> isShowPasswordButtonVisibleSubject = new Subject<bool>();
        private readonly BehaviorSubject<bool> isLoadingSubject = new BehaviorSubject<bool>(false);
        private readonly BehaviorSubject<string> errorMessageSubject = new BehaviorSubject<string>("");
        private readonly BehaviorSubject<bool> isPasswordMaskedSubject = new BehaviorSubject<bool>(true);
        private readonly BehaviorSubject<Email> emailSubject = new BehaviorSubject<Email>(Shared.Email.Empty);
        private readonly BehaviorSubject<Password> passwordSubject = new BehaviorSubject<Password>(Shared.Password.Empty);

        public IObservable<string> Email { get; }
        public IObservable<string> Password { get; }
        public IObservable<bool> HasError { get; }
        public IObservable<bool> IsLoading { get; }
        public IObservable<bool> LoginEnabled { get; }
        public IObservable<ShakeTargets> Shake { get; }
        public IObservable<string> ErrorMessage { get; }
        public IObservable<bool> IsPasswordMasked { get; }
        public IObservable<bool> IsShowPasswordButtonVisible { get; }

        public ViewAction Signup { get; }
        public ViewAction ForgotPassword { get; }

        public LoginViewModel(
            IUserAccessManager userAccessManager,
            IAnalyticsService analyticsService,
            IOnboardingStorage onboardingStorage,
            INavigationService navigationService,
            IErrorHandlingService errorHandlingService,
            ILastTimeUsageStorage lastTimeUsageStorage,
            ITimeService timeService,
            ISchedulerProvider schedulerProvider,
            IRxActionFactory rxActionFactory,
            IInteractorFactory interactorFactory)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(userAccessManager, nameof(userAccessManager));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            Ensure.Argument.IsNotNull(onboardingStorage, nameof(onboardingStorage));
            Ensure.Argument.IsNotNull(errorHandlingService, nameof(errorHandlingService));
            Ensure.Argument.IsNotNull(lastTimeUsageStorage, nameof(lastTimeUsageStorage));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));

            this.timeService = timeService;
            this.userAccessManager = userAccessManager;
            this.analyticsService = analyticsService;
            this.onboardingStorage = onboardingStorage;
            this.errorHandlingService = errorHandlingService;
            this.lastTimeUsageStorage = lastTimeUsageStorage;
            this.schedulerProvider = schedulerProvider;
            this.interactorFactory = interactorFactory;

            var emailObservable = emailSubject.Select(email => email.TrimmedEnd());

            Signup = rxActionFactory.FromAsync(signup);
            ForgotPassword = rxActionFactory.FromAsync(forgotPassword);

            Shake = shakeSubject.AsDriver(this.schedulerProvider);

            Email = emailObservable
                .Select(email => email.ToString())
                .DistinctUntilChanged()
                .AsDriver(this.schedulerProvider);

            Password = passwordSubject
                .Select(password => password.ToString())
                .DistinctUntilChanged()
                .AsDriver(this.schedulerProvider);

            IsLoading = isLoadingSubject
                .DistinctUntilChanged()
                .AsDriver(this.schedulerProvider);

            ErrorMessage = errorMessageSubject
                .DistinctUntilChanged()
                .AsDriver(this.schedulerProvider);

            IsPasswordMasked = isPasswordMaskedSubject
                .DistinctUntilChanged()
                .AsDriver(this.schedulerProvider);

            IsShowPasswordButtonVisible = Password
                .Select(password => password.Length > 1)
                .CombineLatest(isShowPasswordButtonVisibleSubject.AsObservable(), CommonFunctions.And)
                .DistinctUntilChanged()
                .AsDriver(this.schedulerProvider);

            HasError = ErrorMessage
                .Select(string.IsNullOrEmpty)
                .Select(CommonFunctions.Invert)
                .AsDriver(this.schedulerProvider);

            LoginEnabled = emailObservable
                .CombineLatest(
                    passwordSubject.AsObservable(),
                    IsLoading,
                    (email, password, isLoading) => email.IsValid && password.IsValid && !isLoading)
                .DistinctUntilChanged()
                .AsDriver(this.schedulerProvider);
        }

        public override Task Initialize(CredentialsParameter parameter)
        {
            emailSubject.OnNext(parameter.Email);
            passwordSubject.OnNext(parameter.Password);

            return base.Initialize(parameter);
        }

        public void SetEmail(Email email)
            => emailSubject.OnNext(email);

        public void SetPassword(Password password)
            => passwordSubject.OnNext(password);

        public void SetIsShowPasswordButtonVisible(bool visible)
            => isShowPasswordButtonVisibleSubject.OnNext(visible);

        public void Login()
        {
            var shakeTargets = ShakeTargets.None;
            shakeTargets |= emailSubject.Value.IsValid ? ShakeTargets.None : ShakeTargets.Email;
            shakeTargets |= passwordSubject.Value.IsValid ? ShakeTargets.None : ShakeTargets.Password;

            if (shakeTargets != ShakeTargets.None)
            {
                shakeSubject.OnNext(shakeTargets);
                return;
            }

            if (isLoadingSubject.Value) return;

            isLoadingSubject.OnNext(true);
            errorMessageSubject.OnNext("");

            loginDisposable =
                userAccessManager
                    .Login(emailSubject.Value, passwordSubject.Value)
                    .Track(analyticsService.Login, AuthenticationMethod.EmailAndPassword)
                    .Subscribe(_ => onAuthenticated(), onError, onCompleted);
        }

        public void TogglePasswordVisibility()
            => isPasswordMaskedSubject.OnNext(!isPasswordMaskedSubject.Value);

        public void GoogleLogin()
        {
            if (isLoadingSubject.Value) return;

            isLoadingSubject.OnNext(true);

            loginDisposable = View?
                .GetGoogleToken()
                .SelectMany(userAccessManager.LoginWithGoogle)
                .Track(analyticsService.Login, AuthenticationMethod.Google)
                .Subscribe(_ => onAuthenticated(), onError, onCompleted);
        }

        private Task signup()
        {
            if (isLoadingSubject.Value)
                return Task.CompletedTask;

            var parameter = CredentialsParameter.With(emailSubject.Value, passwordSubject.Value);
            return Navigate<SignupViewModel, CredentialsParameter>(parameter);
        }

        private async Task forgotPassword()
        {
            if (isLoadingSubject.Value) return;

            var emailParameter = EmailParameter.With(emailSubject.Value);
            emailParameter = await Navigate<ForgotPasswordViewModel, EmailParameter, EmailParameter>(emailParameter);
            if (emailParameter != null)
                emailSubject.OnNext(emailParameter.Email);
        }

        private async void onAuthenticated()
        {
            lastTimeUsageStorage.SetLogin(timeService.CurrentDateTime);

            onboardingStorage.SetIsNewUser(false);

            interactorFactory.GetCurrentUser().Execute()
                .Select(u => u.Id)
                .Subscribe(analyticsService.SetAppCenterUserId);

            await UIDependencyContainer.Instance.SyncManager.ForceFullSync();

            await Navigate<MainTabBarViewModel>();
        }

        private void onError(Exception exception)
        {
            isLoadingSubject.OnNext(false);
            onCompleted();

            if (errorHandlingService.TryHandleDeprecationError(exception))
                return;

            switch (exception)
            {
                case UnauthorizedException forbidden:
                    errorMessageSubject.OnNext(Resources.IncorrectEmailOrPassword);
                    break;
                case GoogleLoginException googleEx when googleEx.LoginWasCanceled:
                    errorMessageSubject.OnNext("");
                    break;
                default:
                    analyticsService.UnknownLoginFailure.Track(exception.GetType().FullName, exception.Message);
                    analyticsService.TrackAnonymized(exception);
                    errorMessageSubject.OnNext(Resources.GenericLoginError);
                    break;
            }
        }

        private void onCompleted()
        {
            loginDisposable?.Dispose();
            loginDisposable = null;
        }
    }
}
