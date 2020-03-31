using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Toggl.Core.Interactors;
using Toggl.Core.Services;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Views;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class SendFeedbackViewModel : ViewModelWithOutput<bool>
    {
        private readonly IInteractorFactory interactorFactory;

        // Internal States
        private readonly ISubject<bool> isLoadingSubject = new BehaviorSubject<bool>(false);
        private readonly ISubject<Exception> currentErrorSubject = new BehaviorSubject<Exception>(null);

        // Actions
        public ViewAction DismissError { get; }
        public ViewAction Send { get; }

        // Inputs
        public ISubject<string> FeedbackText { get; } = new BehaviorSubject<string>(string.Empty);

        // Outputs
        public IObservable<bool> IsFeedbackEmpty { get; }
        public IObservable<bool> SendEnabled { get; }
        public IObservable<bool> IsLoading { get; }
        public IObservable<Exception> Error { get; }

        private IObservable<bool> isEmptyObservable => FeedbackText.Select(string.IsNullOrEmpty);
        private IObservable<bool> sendingIsEnabledObservable =>
            isEmptyObservable.CombineLatest(
                isLoadingSubject.AsObservable(),
                (isEmpty, isLoading) => !isEmpty && !isLoading);

        public SendFeedbackViewModel(
            INavigationService navigationService,
            IInteractorFactory interactorFactory,
            ISchedulerProvider schedulerProvider,
            IRxActionFactory rxActionFactory)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            this.interactorFactory = interactorFactory;

            IsFeedbackEmpty = isEmptyObservable.DistinctUntilChanged().AsDriver(schedulerProvider);
            SendEnabled = sendingIsEnabledObservable.DistinctUntilChanged().AsDriver(schedulerProvider);

            DismissError = rxActionFactory.FromAction(dismissError);
            Send = rxActionFactory.FromObservable(sendFeedback, sendingIsEnabledObservable);

            IsLoading = isLoadingSubject.AsDriver(false, schedulerProvider);
            Error = currentErrorSubject.AsDriver(default(Exception), schedulerProvider);
        }

        private void dismissError()
        {
            currentErrorSubject.OnNext(null);
        }

        public override async Task<bool> ConfirmCloseRequest()
        {
            var feedbackText = await FeedbackText.FirstAsync();
            if (!string.IsNullOrEmpty(feedbackText))
            {
                var view = View;
                if (view == null)
                    return true;

                return await view
                    .ConfirmDestructiveAction(ActionType.DiscardFeedback);
            }

            return true;
        }

        private IObservable<Unit> sendFeedback()
            => FeedbackText.FirstAsync()
                .Do(_ =>
                {
                    isLoadingSubject.OnNext(true);
                    currentErrorSubject.OnNext(null);
                })
                .SelectMany(text => interactorFactory
                    .SendFeedback(text)
                    .Execute()
                    .Materialize())
                .Do(notification =>
                {
                    switch (notification.Kind)
                    {
                        case NotificationKind.OnError:
                            isLoadingSubject.OnNext(false);
                            currentErrorSubject.OnNext(notification.Exception);
                            break;

                        default:
                            Close(true);
                            break;
                    }
                })
                .SelectUnit();
    }
}
