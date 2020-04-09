using System;
using Toggl.Storage.Onboarding;

namespace Toggl.Storage.Settings
{
    public interface IOnboardingStorage
    {
        IObservable<bool> IsNewUser { get; }
        IObservable<bool> UserSignedUpUsingTheApp { get; }
        IObservable<bool> StartButtonWasTappedBefore { get; }
        IObservable<bool> HasTappedTimeEntry { get; }
        IObservable<bool> HasEditedTimeEntry { get; }
        IObservable<bool> StopButtonWasTappedBefore { get; }
        IObservable<bool> HasSelectedProject { get; }
        IObservable<bool> ProjectOrTagWasAddedBefore { get; }
        IObservable<bool> NavigatedAwayFromMainViewAfterTappingStopButton { get; }
        IObservable<bool> HasTimeEntryBeenContinued { get; }

        void SetCompletedOnboarding();
        void SetIsNewUser(bool isNewUser);
        void SetLastOpened(DateTimeOffset dateString);
        void SetFirstOpened(DateTimeOffset dateTime);
        void SetUserSignedUp();
        void SetNavigatedAwayFromMainViewAfterStopButton();
        void SetTimeEntryContinued();
        void SetCompletedCalendarOnboarding();

        DateTimeOffset? GetLastOpened();
        DateTimeOffset? GetFirstOpened();
        bool CompletedOnboarding();
        bool CompletedCalendarOnboarding();

        void StartButtonWasTapped();
        void TimeEntryWasTapped();
        void ProjectOrTagWasAdded();
        void StopButtonWasTapped();

        void EditedTimeEntry();
        void SelectsProject();

        void SetJanuary2020CampaignWasShown();
        bool WasJanuary2020CampaignShown();

        bool WasDismissed(IDismissable dismissable);
        void Dismiss(IDismissable dismissable);

        void SetDidShowRatingView();
        int NumberOfTimesRatingViewWasShown();
        void SetRatingViewOutcome(RatingViewOutcome outcome, DateTimeOffset dateTime);
        RatingViewOutcome? RatingViewOutcome();
        DateTimeOffset? RatingViewOutcomeTime();

        bool DidShowSiriClipboardInstruction();
        void SetDidShowSiriClipboardInstruction(bool value);

        bool IsFirstTimeConnectingCalendars();
        void SetIsFirstTimeConnectingCalendars();

        void Reset();
    }
}
