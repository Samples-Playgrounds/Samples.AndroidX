using Android.App;
using Android.Content;
using Java.Lang;
using System;
using System.Reactive.Linq;
using Toggl.Core;
using Toggl.Core.Models.Interfaces;
using Toggl.Droid.Extensions;
using Toggl.Shared;
using Uri = Android.Net.Uri;

namespace Toggl.Droid.Helper
{
    public static class PersistentNotificationsHelper
    {
        private static Uri togglMainNavigationUri = Uri.Parse(ApplicationUrls.Main.Default);
        private static int runningTimeEntryNotificationId = 111;
        private static int idleTimerNotificationId = 112;
        private static char dotSeparator = '\u00b7';

        public static IDisposable BindRunningTimeEntry<T>(this T activity,
            NotificationManager notificationManager,
            IObservable<IThreadSafeTimeEntry> runningTimeEntry,
            IObservable<bool> shouldShowRunningTimeEntryNotification)
        where T : Activity
        {
            var runningTimeEntryNotificationSource = runningTimeEntry
                .CombineLatest(shouldShowRunningTimeEntryNotification,
                    (te, shouldShowNotification) => shouldShowNotification ? te : null);

            return runningTimeEntryNotificationSource
                .Subscribe(te => updateRunningNotification(te, activity, notificationManager));
        }

        public static IDisposable BindIdleTimer<T>(this T activity,
            NotificationManager notificationManager,
            IObservable<bool> isTimeEntryRunning,
            IObservable<bool> shouldShowStoppedTimeEntryNotification)
        where T : Activity
        {
            var idleTimerNotificationSource = isTimeEntryRunning
                .CombineLatest(shouldShowStoppedTimeEntryNotification,
                    (isRunning, shouldShowNotification) => shouldShowNotification && !isRunning);

            return idleTimerNotificationSource
                .Subscribe(shouldShow => updateIdleTimerNotification(shouldShow, activity, notificationManager));
        }

        private static void updateRunningNotification(IThreadSafeTimeEntry timeEntryViewModel, Activity activity, NotificationManager notificationManager)
        {
            if (notificationManager == null) return;

            if (timeEntryViewModel != null)
            {
                var startTime = timeEntryViewModel.Start.ToUnixTimeMilliseconds();
                var timeEntryDescription = string.IsNullOrEmpty(timeEntryViewModel.Description)
                    ? Resources.NoDescription
                    : timeEntryViewModel.Description;
                var projectDetails = extractProjectDetails(timeEntryViewModel);

                var notification = activity.CreateNotificationBuilderWithDefaultChannel(notificationManager)
                    .SetShowWhen(true)
                    .SetUsesChronometer(true)
                    .SetAutoCancel(false)
                    .SetOngoing(true)
                    .SetContentTitle(timeEntryDescription)
                    .SetContentText(projectDetails)
                    .SetWhen(startTime)
                    .SetContentIntent(getIntentFor(activity))
                    .SetSmallIcon(Resource.Drawable.ic_icon_running)
                    .Build();

                notificationManager.Notify(runningTimeEntryNotificationId, notification);
            }
            else
            {
                notificationManager.Cancel(runningTimeEntryNotificationId);
            }
        }

        private static void updateIdleTimerNotification(bool shouldShow, Activity activity, NotificationManager notificationManager)
        {
            if (notificationManager == null) return;

            if (shouldShow)
            {
                var notification = activity.CreateNotificationBuilderWithDefaultChannel(notificationManager)
                    .SetShowWhen(true)
                    .SetAutoCancel(false)
                    .SetOngoing(true)
                    .SetContentTitle(Resources.AppTitle)
                    .SetContentText(Resources.IdleTimerNotification)
                    .SetContentIntent(getIntentFor(activity))
                    .SetSmallIcon(Resource.Drawable.ic_icon_notrunning)
                    .Build();

                notificationManager.Notify(idleTimerNotificationId, notification);
            }
            else
            {
                notificationManager.Cancel(idleTimerNotificationId);
            }
        }

        private static string extractProjectDetails(IThreadSafeTimeEntry timeEntryViewModel)
        {
            if (timeEntryViewModel.ProjectId == null)
            {
                return Resources.NoProject;
            }

            var projectDetails = new StringBuilder(timeEntryViewModel.Project.Name);
            if (timeEntryViewModel.TaskId != null)
            {
                projectDetails.Append($": {timeEntryViewModel.Task.Name}");
            }

            if (timeEntryViewModel.Project.ClientId != null)
            {
                projectDetails.Append($" {dotSeparator} {timeEntryViewModel.Project.Client.Name}");
            }
            return projectDetails.ToString();
        }


        private static PendingIntent getIntentFor(Activity activity)
        {
            var notificationIntent = activity.PackageManager.GetLaunchIntentForPackage(activity.PackageName);
            notificationIntent.SetPackage(null);
            notificationIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ResetTaskIfNeeded);
            return PendingIntent.GetActivity(activity, 0, notificationIntent, 0);
        }
    }
}
