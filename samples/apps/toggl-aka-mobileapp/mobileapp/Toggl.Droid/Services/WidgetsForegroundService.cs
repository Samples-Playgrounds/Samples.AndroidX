using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using AndroidX.Core.App;
using Toggl.Core;
using Toggl.Core.Analytics;
using Toggl.Core.Extensions;
using Toggl.Core.Interactors;
using Toggl.Droid.Helper;
using Toggl.Droid.Widgets;
using static Toggl.Droid.Services.JobServicesConstants;
using static Toggl.Droid.Helper.NotificationsConstants;
using static Toggl.Droid.Widgets.WidgetsConstants;
using Toggl.Core.Exceptions;

namespace Toggl.Droid.Services
{
    [Service(Permission = "android.permission.BIND_JOB_SERVICE", Exported = true)]
    public sealed class WidgetsForegroundService : IntentService
    {
        private IInteractorFactory interactorFactory => AndroidDependencyContainer.Instance.InteractorFactory;
        private ITimeService timeService => AndroidDependencyContainer.Instance.TimeService;

        protected override void OnHandleIntent(Intent intent)
        {
            if (OreoApis.AreAvailable)
            {
                var notificationManager = GetSystemService(NotificationService) as NotificationManager;
                var channel = new NotificationChannel(DefaultChannelId, DefaultChannelName, NotificationImportance.Low);
                channel.Description = DefaultChannelDescription;
                channel.EnableVibration(false);
                channel.SetVibrationPattern(NoNotificationVibrationPattern);
                notificationManager.CreateNotificationChannel(channel);
            }

            var notificationBuilder = new NotificationCompat.Builder(this, DefaultChannelId);
            notificationBuilder.SetVibrate(NoNotificationVibrationPattern);

            StartForeground(WidgetForegroundServiceJobId, notificationBuilder.Build());

            onHandleIntentAsync(intent);
        }

        private async void onHandleIntentAsync(Intent intent)
        {
            AndroidDependencyContainer.EnsureInitialized(Application.Context);
            AndroidDependencyContainer.Instance.WidgetsService.Start();

            var action = intent.Action;

            switch (action)
            {
                case StartTimeEntryAction:
                    await handleStartTimeEntry();
                    break;
                case StopRunningTimeEntryAction:
                    await handleStopRunningTimeEntry();
                    break;
                case SuggestionTapped:
                    await continueTimeEntryFromSuggestion(intent);
                    break;
                default:
                    throw new InvalidOperationException($"Cannot handle intent with action {action}");
            }

            StopForeground(true);
        }

        private async Task handleStartTimeEntry()
        {
            var now = timeService.CurrentDateTime;
            var workspaceId = (await interactorFactory.GetDefaultWorkspace().Execute()).Id;
            var prototype = "".AsTimeEntryPrototype(now, workspaceId);
            await interactorFactory.CreateTimeEntry(prototype, TimeEntryStartOrigin.Widget).Execute();
        }

        private async Task handleStopRunningTimeEntry()
        {
            var now = timeService.CurrentDateTime;

            try
            {
                await interactorFactory.StopTimeEntry(now, TimeEntryStopOrigin.Widget).Execute();
            }
            catch(NoRunningTimeEntryException)
            {
                /* There is no need to handle this because stopping a time entry should be
                 * idempotent operation. This happens only when user is tapping the widget's
                 * stop button multiple times in quick succession.
                 */
            }
        }

        private async Task continueTimeEntryFromSuggestion(Intent intent)
        {
            var index = intent.GetIntExtra(TappedSuggestionIndex, 0);

            var suggestions = WidgetSuggestionItem.SuggestionsFromSharedPreferences().ToList();
            var suggestion = suggestions[index];

            var now = timeService.CurrentDateTime;

            var timeEntryPrototype = suggestion.Description.AsTimeEntryPrototype(
                startTime: now,
                workspaceId: suggestion.WorkspaceId,
                projectId: suggestion.ProjectId,
                taskId: suggestion.TaskId,
                isBillable:suggestion.IsBillable,
                tagIds: suggestion.TagsIds);

            await interactorFactory.CreateTimeEntry(timeEntryPrototype, TimeEntryStartOrigin.Widget).Execute();
        }
    }
}
