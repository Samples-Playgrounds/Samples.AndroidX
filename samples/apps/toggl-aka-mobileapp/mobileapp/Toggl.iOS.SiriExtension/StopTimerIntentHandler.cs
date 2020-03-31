using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Toggl.iOS.Shared;
using Toggl.iOS.Shared.Analytics;
using Toggl.iOS.Shared.Exceptions;
using Toggl.iOS.Shared.Extensions;
using Toggl.iOS.Shared.Models;
using System.Reactive.Threading.Tasks;
using Toggl.iOS.Intents;
using Toggl.Networking;
using Toggl.Shared;
using Toggl.Shared.Models;

namespace SiriExtension
{
    public class StopTimerIntentHandler : StopTimerIntentHandling
    {
        private ITogglApi togglAPI;
        private static ITimeEntry runningEntry;
        private const string stopTimerActivityType = "StopTimer";

        public StopTimerIntentHandler(ITogglApi togglAPI)
        {
            this.togglAPI = togglAPI;
        }

        public override void ConfirmStopTimer(StopTimerIntent intent, Action<StopTimerIntentResponse> completion)
        {
            if (togglAPI == null)
            {
                var userActivity = new NSUserActivity(stopTimerActivityType);
                userActivity.SetResponseText(Resources.SiriShortcutLoginToUseShortcut);
                completion(new StopTimerIntentResponse(StopTimerIntentResponseCode.FailureNoApiToken, userActivity));
                return;
            }

            togglAPI.TimeEntries.GetAll()
                .ToObservable()
                .Select(getRunningTimeEntry)
                .Subscribe(
                    runningTE =>
                    {
                        runningEntry = runningTE;
                        var userActivity = new NSUserActivity(stopTimerActivityType);
                        userActivity.SetEntryDescription(runningTE.Description);
                        completion(new StopTimerIntentResponse(StopTimerIntentResponseCode.Ready, userActivity));
                    },
                    exception =>
                    {
                        SharedStorage.Instance.AddSiriTrackingEvent(SiriTrackingEvent.Error(exception.Message));
                        completion(responseFromException(exception));
                    });
        }

        public override void HandleStopTimer(StopTimerIntent intent, Action<StopTimerIntentResponse> completion)
        {
            SharedStorage.Instance.SetNeedsSync(true);

            stopTimeEntry(runningEntry)
                .Subscribe(
                    stoppedTimeEntry =>
                    {
                        var timeSpan = TimeSpan.FromSeconds(stoppedTimeEntry.Duration ?? 0);

                        var response = string.IsNullOrEmpty(stoppedTimeEntry.Description)
                            ? StopTimerIntentResponse.SuccessWithEmptyDescriptionIntentResponseWithEntryDurationString(
                                durationStringForTimeSpan(timeSpan))
                            : StopTimerIntentResponse.SuccessIntentResponseWithEntryDescription(
                                stoppedTimeEntry.Description, durationStringForTimeSpan(timeSpan)
                            );
                        response.EntryStart = stoppedTimeEntry.Start.ToUnixTimeSeconds();
                        response.EntryDuration = stoppedTimeEntry.Duration;

                        SharedStorage.Instance.AddSiriTrackingEvent(SiriTrackingEvent.StopTimer());

                        completion(response);
                    },
                    exception =>
                    {
                        SharedStorage.Instance.AddSiriTrackingEvent(SiriTrackingEvent.Error(exception.Message));
                        completion(responseFromException(exception));
                    }
                );
        }

        private string durationStringForTimeSpan(TimeSpan timeSpan)
        {
            if (timeSpan.Hours == 0 && timeSpan.Minutes == 0)
            {
                return string.Format(Resources.SiriDurationWithSeconds, timeSpan.Seconds);
            }

            if (timeSpan.Hours == 0)
            {
                return string.Format(Resources.SiriDurationWithMinutesAndSeconds, timeSpan.Minutes, timeSpan.Seconds);
            }

            if (timeSpan.Minutes == 0)
            {
                return string.Format(Resources.SiriDurationWithHoursAndSeconds, timeSpan.Hours, timeSpan.Seconds);
            }

            return string.Format(Resources.SiriDurationWithHoursMinutesAndSeconds, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
        }

        private ITimeEntry getRunningTimeEntry(IList<ITimeEntry> timeEntries)
        {
            try
            {
                var runningTE = timeEntries.Where(te => te.Duration == null).First();
                return runningTE;
            }
            catch
            {
                throw new NoRunningEntryException();
            }
        }

        private IObservable<ITimeEntry> stopTimeEntry(ITimeEntry timeEntry)
        {
            var duration = (long)(DateTime.Now - timeEntry.Start).TotalSeconds;
            return togglAPI.TimeEntries.Update(
                TimeEntry.From(timeEntry).With(duration)
            ).ToObservable();
        }

        private StopTimerIntentResponse responseFromException(Exception exception)
        {
            var userActivity = new NSUserActivity(stopTimerActivityType);
            if (exception is NoRunningEntryException)
            {
                userActivity.SetResponseText(Resources.SiriNoCurrentEntryRunning);
                return new StopTimerIntentResponse(StopTimerIntentResponseCode.FailureNoTimerRunning, userActivity);
            }

            userActivity.SetResponseText(Resources.SiriShortcutOpenTheAppToSync);
            return new StopTimerIntentResponse(StopTimerIntentResponseCode.Failure, userActivity);
        }
    }
}
