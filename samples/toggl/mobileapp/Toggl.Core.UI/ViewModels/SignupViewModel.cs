using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.Exceptions;
using Toggl.Core.Extensions;
using Toggl.Core.Interactors;
using Toggl.Core.Interactors.Location;
using Toggl.Core.Interactors.Timezones;
using Toggl.Core.Login;
using Toggl.Core.Services;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
using Toggl.Networking.Exceptions;
using Toggl.Networking.Network;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Shared.Models;
using Toggl.Storage.Settings;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class SignupViewModel : ViewModelWithInput<CredentialsParameter>
    {
        [Flags]
        public enum ShakeTargets
        {
            None = 0,
            Email = 1 << 0,
            Password = 1 << 1,
            Country = 1 << 2
        }

        private readonly IApiFactory apiFactory;
        private readonly IUserAccessManager userAccessManager;
        private readonly IAnalyticsService analyticsService;
        private readonly IOnboardingStorage onboardingStorage;
        private readonly IErrorHandlingService errorHandlingService;
        private readonly ILastTimeUsageStorage lastTimeUsageStorage;
        private readonly ITimeService timeService;
        private readonly ISchedulerProvider schedulerProvider;
        private readonly IPlatformInfo platformInfo;

        private IDisposable getCountrySubscription;
        private IDisposable signupDisposable;
        private bool termsOfServiceAccepted;
        private List<ICountry> allCountries;
        private long? countryId;
        private string timezone;

        private readonly Subject<ShakeTargets> shakeSubject = new Subject<ShakeTargets>();
        private readonly Subject<bool> isShowPasswordButtonVisibleSubject = new Subject<bool>();
        private readonly BehaviorSubject<bool> isLoadingSubject = new BehaviorSubject<bool>(false);
        private readonly BehaviorSubject<string> errorMessageSubject = new BehaviorSubject<string>(string.Empty);
        private readonly BehaviorSubject<bool> isPasswordMaskedSubject = new BehaviorSubject<bool>(true);
        private readonly BehaviorSubject<Email> emailSubject = new BehaviorSubject<Email>(Shared.Email.Empty);
        private readonly BehaviorSubject<Password> passwordSubject = new BehaviorSubject<Password>(Shared.Password.Empty);
        private readonly BehaviorSubject<string> countryNameSubject = new BehaviorSubject<string>(Resources.SelectCountry);
        private readonly BehaviorSubject<bool> isCountryErrorVisibleSubject = new BehaviorSubject<bool>(false);
        private readonly Subject<Unit> successfulSignupSubject = new Subject<Unit>();
        private readonly CompositeDisposable disposeBag = new CompositeDisposable();

        public IObservable<string> CountryButtonTitle { get; }
        public IObservable<bool> IsCountryErrorVisible { get; }
        public IObservable<string> Email { get; }
        public IObservable<string> Password { get; }
        public IObservable<bool> HasError { get; }
        public IObservable<bool> IsLoading { get; }
        public IObservable<bool> SignupEnabled { get; }
        public IObservable<ShakeTargets> Shake { get; }
        public IObservable<string> ErrorMessage { get; }
        public IObservable<bool> IsPasswordMasked { get; }
        public IObservable<bool> IsShowPasswordButtonVisible { get; }
        public IObservable<Unit> SuccessfulSignup { get; }

        public ViewAction Login { get; }
        public ViewAction Signup { get; }
        public ViewAction GoogleSignup { get; }
        public ViewAction PickCountry { get; }

        public SignupViewModel(
            IApiFactory apiFactory,
            IUserAccessManager userAccessManager,
            IAnalyticsService analyticsService,
            IOnboardingStorage onboardingStorage,
            INavigationService navigationService,
            IErrorHandlingService errorHandlingService,
            ILastTimeUsageStorage lastTimeUsageStorage,
            ITimeService timeService,
            ISchedulerProvider schedulerProvider,
            IRxActionFactory rxActionFactory,
            IPlatformInfo platformInfo)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(apiFactory, nameof(apiFactory));
            Ensure.Argument.IsNotNull(userAccessManager, nameof(userAccessManager));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            Ensure.Argument.IsNotNull(onboardingStorage, nameof(onboardingStorage));
            Ensure.Argument.IsNotNull(errorHandlingService, nameof(errorHandlingService));
            Ensure.Argument.IsNotNull(lastTimeUsageStorage, nameof(lastTimeUsageStorage));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));
            Ensure.Argument.IsNotNull(platformInfo, nameof(platformInfo));

            this.apiFactory = apiFactory;
            this.userAccessManager = userAccessManager;
            this.analyticsService = analyticsService;
            this.onboardingStorage = onboardingStorage;
            this.errorHandlingService = errorHandlingService;
            this.lastTimeUsageStorage = lastTimeUsageStorage;
            this.timeService = timeService;
            this.schedulerProvider = schedulerProvider;
            this.platformInfo = platformInfo;

            Login = rxActionFactory.FromAsync(login);
            Signup = rxActionFactory.FromAsync(signup);
            GoogleSignup = rxActionFactory.FromAsync(googleSignup);
            PickCountry = rxActionFactory.FromAsync(pickCountry);

            var emailObservable = emailSubject.Select(email => email.TrimmedEnd());

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

            IsCountryErrorVisible = isCountryErrorVisibleSubject
                .DistinctUntilChanged()
                .AsDriver(this.schedulerProvider);

            ErrorMessage = errorMessageSubject
                .DistinctUntilChanged()
                .AsDriver(this.schedulerProvider);

            CountryButtonTitle = countryNameSubject
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

            SignupEnabled = emailObservable
                .CombineLatest(
                    passwordSubject.AsObservable(),
                    IsLoading,
                    countryNameSubject.AsObservable(),
                    (email, password, isLoading, countryName) => email.IsValid && password.IsValid && !isLoading && (countryName != Resources.SelectCountry))
                .DistinctUntilChanged()
                .AsDriver(this.schedulerProvider);

            SuccessfulSignup = successfulSignupSubject
                .AsDriver(this.schedulerProvider);
        }

        public void SetEmail(Email email)
            => emailSubject.OnNext(email);

        public void SetPassword(Password password)
            => passwordSubject.OnNext(password);

        public void SetIsShowPasswordButtonVisible(bool visible)
            => isShowPasswordButtonVisibleSubject.OnNext(visible);

        public override async Task Initialize(CredentialsParameter parameter)
        {
            await base.Initialize(parameter);

            emailSubject.OnNext(parameter.Email);
            passwordSubject.OnNext(parameter.Password);

            allCountries = await new GetAllCountriesInteractor().Execute();

            var api = apiFactory.CreateApiWith(Credentials.None);
            getCountrySubscription = new GetCurrentLocationInteractor(api)
                .Execute()
                .Select(location => allCountries.First(country => country.CountryCode == location.CountryCode))
                .Subscribe(
                    setCountryIfNeeded,
                    _ => setCountryErrorIfNeeded(),
                    () =>
                    {
                        getCountrySubscription?.Dispose();
                        getCountrySubscription = null;
                    }
                );
        }

        public override void ViewDisappeared()
        {
            base.ViewDisappeared();
            disposeBag?.Dispose();
        }

        private void setCountryIfNeeded(ICountry country)
        {
            if (countryId.HasValue) return;
            countryId = country.Id;
            countryNameSubject.OnNext(country.Name);
        }

        private void setCountryErrorIfNeeded()
        {
            if (countryId.HasValue) return;

            isCountryErrorVisibleSubject.OnNext(true);
        }

        private async Task signup()
        {
            var shakeTargets = ShakeTargets.None;
            if (!emailSubject.Value.IsValid)
            {
                shakeTargets |= ShakeTargets.Email;
            }
            if (!passwordSubject.Value.IsValid)
            {
                shakeTargets |= ShakeTargets.Password;
            }
            if (!countryId.HasValue)
            {
                shakeTargets |= ShakeTargets.Country;
            }

            if (shakeTargets != ShakeTargets.None)
            {
                shakeSubject.OnNext(shakeTargets);
                return;
            }

            await requestAcceptanceOfTermsAndConditionsIfNeeded();

            if (!termsOfServiceAccepted || isLoadingSubject.Value) return;

            isLoadingSubject.OnNext(true);
            errorMessageSubject.OnNext(string.Empty);

            var supportedTimezonesObs = new GetSupportedTimezonesInteractor().Execute();
            signupDisposable = supportedTimezonesObs
                .Select(supportedTimezones => supportedTimezones.FirstOrDefault(tz => platformInfo.TimezoneIdentifier == tz))
                .SelectMany(timezone
                    => userAccessManager
                        .SignUp(
                            emailSubject.Value,
                            passwordSubject.Value,
                            termsOfServiceAccepted,
                             (int)countryId.Value,
                            timezone)
                )
                .Track(analyticsService.SignUp, AuthenticationMethod.EmailAndPassword)
                .Do(_ =>
                {
                    var password = passwordSubject.Value;
                    if (!password.IsValid)
                        return;

                    analyticsService.Track(new SignupPasswordComplexityEvent(password));
                })
                .Subscribe(_ => onAuthenticated(), onError, onCompleted);
        }

        private async void onAuthenticated()
        {
            successfulSignupSubject.OnNext(Unit.Default);

            lastTimeUsageStorage.SetLogin(timeService.CurrentDateTime);

            onboardingStorage.SetIsNewUser(true);
            onboardingStorage.SetUserSignedUp();

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
                    errorMessageSubject.OnNext(string.Empty);
                    break;
                case EmailIsAlreadyUsedException _:
                    errorMessageSubject.OnNext(Resources.EmailIsAlreadyUsedError);
                    break;
                default:
                    analyticsService.UnknownSignUpFailure.Track(exception.GetType().FullName, exception.Message);
                    analyticsService.TrackAnonymized(exception);
                    errorMessageSubject.OnNext(Resources.GenericSignUpError);
                    break;
            }
        }

        private void onCompleted()
        {
            signupDisposable?.Dispose();
            signupDisposable = null;
        }

        private async Task googleSignup()
        {
            if (!countryId.HasValue)
            {
                shakeSubject.OnNext(ShakeTargets.Country);
                return;
            }

            await requestAcceptanceOfTermsAndConditionsIfNeeded();

            if (!termsOfServiceAccepted || isLoadingSubject.Value) return;

            isLoadingSubject.OnNext(true);
            errorMessageSubject.OnNext(string.Empty);

            signupDisposable = View.GetGoogleToken()
                .SelectMany(googleToken => userAccessManager
                    .SignUpWithGoogle(googleToken, termsOfServiceAccepted, (int)countryId.Value, timezone))
                .Track(analyticsService.SignUp, AuthenticationMethod.Google)
                .Subscribe(_ => onAuthenticated(), onError, onCompleted);
        }

        public void TogglePasswordVisibility()
            => isPasswordMaskedSubject.OnNext(!isPasswordMaskedSubject.Value);

        private async Task pickCountry()
        {
            getCountrySubscription?.Dispose();
            getCountrySubscription = null;

            var selectedCountryId = await Navigate<SelectCountryViewModel, long?, long?>(countryId);

            if (selectedCountryId == null)
            {
                setCountryErrorIfNeeded();
                return;
            }

            var selectedCountry = allCountries
                .Single(country => country.Id == selectedCountryId.Value);

            isCountryErrorVisibleSubject.OnNext(false);
            countryId = selectedCountry.Id;
            countryNameSubject.OnNext(selectedCountry.Name);
        }

        private Task login()
        {
            if (isLoadingSubject.Value)
                return Task.CompletedTask;

            var parameter = CredentialsParameter.With(emailSubject.Value, passwordSubject.Value);
            return Navigate<LoginViewModel, CredentialsParameter>(parameter);
        }

        private async Task<bool> requestAcceptanceOfTermsAndConditionsIfNeeded()
        {
            if (termsOfServiceAccepted)
                return true;

            termsOfServiceAccepted = await Navigate<TermsOfServiceViewModel, bool>();
            return termsOfServiceAccepted;
        }
    }
}
