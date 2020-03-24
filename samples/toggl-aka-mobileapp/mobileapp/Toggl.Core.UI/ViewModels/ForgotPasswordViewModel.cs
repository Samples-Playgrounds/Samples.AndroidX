using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.Extensions;
using Toggl.Core.Login;
using Toggl.Core.Services;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.Extensions;
using Toggl.Networking.Exceptions;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class ForgotPasswordViewModel : ViewModel<EmailParameter, EmailParameter>
    {
        private readonly ITimeService timeService;
        private readonly IUserAccessManager userAccessManager;
        private readonly IAnalyticsService analyticsService;
        private readonly IRxActionFactory rxActionFactory;

        private readonly TimeSpan delayAfterPassordReset = TimeSpan.FromSeconds(4);
        private readonly ISubject<Unit> passwordResetWithInvalidEmailSubject = new Subject<Unit>();

        public BehaviorSubject<Email> Email { get; } = new BehaviorSubject<Email>(Shared.Email.Empty);
        public IObservable<bool> EmailValid { get; }
        public IObservable<string> ErrorMessage { get; }
        public IObservable<bool> PasswordResetSuccessful { get; }
        public IObservable<Unit> PasswordResetWithInvalidEmail { get; }

        public ViewAction Reset { get; }

        public ForgotPasswordViewModel(
            ITimeService timeService,
            IUserAccessManager userAccessManager,
            IAnalyticsService analyticsService,
            ISchedulerProvider schedulerProvider,
            INavigationService navigationService,
            IRxActionFactory rxActionFactory)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(userAccessManager, nameof(userAccessManager));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            this.timeService = timeService;
            this.userAccessManager = userAccessManager;
            this.analyticsService = analyticsService;
            this.rxActionFactory = rxActionFactory;

            Reset = rxActionFactory.FromObservable(reset);
            PasswordResetWithInvalidEmail = passwordResetWithInvalidEmailSubject.AsDriver(schedulerProvider);

            var resetActionStartedObservable = Reset
                .Executing
                .Where(executing => executing)
                .Select(_ => (Exception)null);

            ErrorMessage = Reset.Errors
                .Merge(resetActionStartedObservable)
                .Select(toErrorString)
                .StartWith("")
                .DistinctUntilChanged();

            PasswordResetSuccessful = Reset.Elements
                .Select(_ => true)
                .StartWith(false)
                .AsDriver(schedulerProvider);

            EmailValid = Email
                .Select(email => email.IsValid)
                .DistinctUntilChanged();
        }

        public override Task Initialize(EmailParameter parameter)
        {
            Email.OnNext(parameter.Email);

            return base.Initialize(parameter);
        }

        private IObservable<Unit> reset()
        {
            if (!Email.Value.IsValid)
            {
                passwordResetWithInvalidEmailSubject.OnNext(Unit.Default);
                return Observable.Empty<Unit>();
            }

            return userAccessManager.ResetPassword(Email.Value)
                .Track(analyticsService.ResetPassword)
                .SelectUnit()
                .Do(closeWithDelay);
        }

        private void closeWithDelay()
        {
            timeService.RunAfterDelay(delayAfterPassordReset, () => { CloseWithDefaultResult(); });
        }

        public override Task<bool> CloseWithDefaultResult()
        {
            Close(EmailParameter.With(Email.Value));
            return Task.FromResult(true);
        }

        private string toErrorString(Exception exception)
        {
            switch (exception)
            {
                case BadRequestException _:
                    return Resources.PasswordResetEmailDoesNotExistError;

                case OfflineException _:
                    return Resources.PasswordResetOfflineError;

                case ApiException apiException:
                    return apiException.LocalizedApiErrorMessage;

                case null:
                    return string.Empty;

                default:
                    return Resources.PasswordResetGeneralError;
            }
        }
    }
}
