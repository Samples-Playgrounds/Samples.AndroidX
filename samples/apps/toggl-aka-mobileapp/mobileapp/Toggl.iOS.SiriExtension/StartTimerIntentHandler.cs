using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using Toggl.iOS.Shared;
using Toggl.iOS.Shared.Analytics;
using Toggl.iOS.Shared.Extensions;
using Toggl.iOS.Shared.Models;
using System.Reactive.Threading.Tasks;
using Toggl.iOS.Intents;
using Toggl.Networking;
using Toggl.Shared;

namespace SiriExtension
{
    public class StartTimerIntentHandler : StartTimerIntentHandling
    {
        private ITogglApi togglAPI;
        private const string startTimerActivityType = "StartTimer";

        public StartTimerIntentHandler(ITogglApi togglAPI)
        {
            this.togglAPI = togglAPI;
        }

        public override void ConfirmStartTimer(StartTimerIntent intent, Action<StartTimerIntentResponse> completion)
        {
            if (togglAPI == null)
            {
                var userActivity = new NSUserActivity(startTimerActivityType);
                userActivity.SetResponseText(Resources.SiriShortcutLoginToUseShortcut);
                completion(new StartTimerIntentResponse(StartTimerIntentResponseCode.FailureNoApiToken, userActivity));
                return;
            }

            completion(new StartTimerIntentResponse(StartTimerIntentResponseCode.Ready, null));
        }

        public override void HandleStartTimer(StartTimerIntent intent, Action<StartTimerIntentResponse> completion)
        {
            var timeEntry = createTimeEntry(intent);
            togglAPI.TimeEntries.Create(timeEntry).ToObservable().Subscribe(te =>
            {
                SharedStorage.Instance.SetNeedsSync(true);
                SharedStorage.Instance.AddSiriTrackingEvent(SiriTrackingEvent.StartTimer(te));

                var response = new StartTimerIntentResponse(StartTimerIntentResponseCode.Success, null);
                completion(response);
            }, exception =>
            {
                SharedStorage.Instance.AddSiriTrackingEvent(SiriTrackingEvent.Error(exception.Message));
                var userActivity = new NSUserActivity(startTimerActivityType);
                userActivity.SetResponseText(Resources.SomethingWentWrongTryAgain);
                completion(new StartTimerIntentResponse(StartTimerIntentResponseCode.Failure, userActivity));
            });
        }

        private TimeEntry createTimeEntry(StartTimerIntent intent)
        {
            var workspaceId = intent.Workspace == null ? SharedStorage.Instance.GetDefaultWorkspaceId() : (long)Convert.ToDouble(intent.Workspace.Identifier);

            if (string.IsNullOrEmpty(intent.EntryDescription))
            {
                return new TimeEntry(workspaceId, null, null, false, DateTimeOffset.Now, null, "", new long[0], (long)SharedStorage.Instance.GetUserId(), 0, null, DateTimeOffset.Now);
            }

            return new TimeEntry(
                workspaceId,
                stringToLong(intent.ProjectId?.Identifier),
                null,
                intent.Billable == null ? false : intent.Billable.Identifier == "True",
                DateTimeOffset.Now,
                null,
                intent.EntryDescription,
                intent.Tags == null ? new long[0] : stringToLongCollection(intent.Tags.Select(tag => tag.Identifier)),
                (long)SharedStorage.Instance.GetUserId(),
                0,
                null,
                DateTimeOffset.Now
            );
        }

        private long? stringToLong(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            return (long)Convert.ToDouble(str);
        }

        private IEnumerable<long> stringToLongCollection(IEnumerable<string> strings)
        {
            if (strings.Count() == 0)
                return new long[0];

            return strings.Select(stringToLong).Cast<long>();
        }
    }
}
