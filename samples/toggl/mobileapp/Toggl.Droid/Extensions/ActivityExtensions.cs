using Android.App;
using Android.App.Job;
using Android.Content;
using Android.Util;
using Android.Views;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using AndroidX.Core.App;
using Toggl.Core.UI.Views;
using Toggl.Droid.Helper;
using Toggl.Droid.Services;
using Toggl.Droid.Views;
using Toggl.Shared.Extensions;
using static Toggl.Droid.Helper.NotificationsConstants;

namespace Toggl.Droid.Extensions
{
    public static class ActivityExtensions
    {
        public static (int widthPixels, int heightPixels, bool isLargeScreen) GetMetrics(this Activity activity, Context context = null)
        {
            const int largeScreenThreshold = 360;

            context = context ?? activity.ApplicationContext;

            var displayMetrics = new DisplayMetrics();
            activity.WindowManager.DefaultDisplay.GetMetrics(displayMetrics);

            var isLargeScreen = displayMetrics.WidthPixels > largeScreenThreshold.DpToPixels(context);

            return (displayMetrics.WidthPixels, displayMetrics.HeightPixels, isLargeScreen);
        }

        public static JobInfo CreateBackgroundSyncJobInfo(this Context context, long periodicity)
        {
            var javaClass = JavaUtils.ToClass<BackgroundSyncJobSchedulerService>();
            var component = new ComponentName(context, javaClass);

            var builder = new JobInfo.Builder(JobServicesConstants.BackgroundSyncJobServiceJobId, component)
                .SetRequiredNetworkType(NetworkType.Any)
                .SetPeriodic(periodicity)
                .SetPersisted(true);

            var jobInfo = builder.Build();
            return jobInfo;
        }

        public static void SetDefaultDialogLayout(this Window window, Activity activity, Context context, int heightDp)
        {
            const int smallScreenWidth = 312;
            const int largeScreenMargins = 72;

            var (widthPixels, heightPixels, isLargeScreen) = activity.GetMetrics(context);

            var width = isLargeScreen
                ? widthPixels - largeScreenMargins.DpToPixels(context)
                : smallScreenWidth.DpToPixels(context);

            var height = heightDp >= 0
                ? heightDp.DpToPixels(context)
                : heightDp;

            window.SetLayout(width, height);
        }

        public static NotificationCompat.Builder CreateNotificationBuilderWithDefaultChannel(this Context context,
            NotificationManager notificationManager)
        {
            if (OreoApis.AreAvailable)
            {
                var channel = new NotificationChannel(DefaultChannelId, DefaultChannelName, NotificationImportance.Low);
                channel.Description = DefaultChannelDescription;
                channel.EnableVibration(false);
                channel.SetVibrationPattern(NoNotificationVibrationPattern);
                notificationManager.CreateNotificationChannel(channel);
            }

            var notificationBuilder = new NotificationCompat.Builder(context, DefaultChannelId);
            notificationBuilder.SetVibrate(NoNotificationVibrationPattern);

            return notificationBuilder;
        }

        public static void CancelAllNotifications(this Context context)
        {
            var notificationManager = context.GetSystemService(Context.NotificationService) as NotificationManager;
            notificationManager?.CancelAll();
        }

        public static IObservable<bool> ShowConfirmationDialog(this Activity activity, string title, string message, string confirmButtonText, string dismissButtonText)
        {
            return Observable.Create<bool>(observer =>
            {
                void showDialogIfActivityIsThere()
                {
                    if (activity.IsFinishing)
                    {
                        observer.CompleteWith(false);
                        return;
                    }

                    var builder = new AlertDialog.Builder(activity)
                        .SetMessage(message)
                        .SetPositiveButton(confirmButtonText, (s, e) => observer.CompleteWith(true));

                    if (!string.IsNullOrWhiteSpace(title))
                    {
                        builder = builder.SetTitle(title);
                    }

                    if (!string.IsNullOrEmpty(dismissButtonText))
                    {
                        builder = builder.SetNegativeButton(dismissButtonText, (s, e) => observer.CompleteWith(false));
                    }

                    var dialog = builder.Create();
                    dialog.Show();

                    dialog.CancelEvent += (s, e) => observer.CompleteWith(false);
                }

                activity.RunOnUiThread(showDialogIfActivityIsThere);

                return Disposable.Empty;
            });
        }

        public static IObservable<T> ShowSelectionDialog<T>(this Activity activity, string title, IEnumerable<SelectOption<T>> options, int initialSelectionIndex = 0)
        {
            return Observable.Create<T>(observer =>
            {
                var dialog = new ListSelectionDialog<T>(activity, title, options, initialSelectionIndex, observer.CompleteWith);
                activity.RunOnUiThread(dialog.Show);
                return Disposable.Empty;
            });
        }

        public static IObservable<bool> ShowDestructiveActionConfirmationDialog(this Activity activity, ActionType type, params object[] formatArguments)
        {
            switch (type)
            {
                case ActionType.DiscardNewTimeEntry:
                    return ShowConfirmationDialog(activity, null, Shared.Resources.DiscardThisTimeEntry, Shared.Resources.Discard, Shared.Resources.Cancel);
                case ActionType.DiscardEditingChanges:
                    return ShowConfirmationDialog(activity, null, Shared.Resources.DiscardEditingChanges, Shared.Resources.Discard, Shared.Resources.ContinueEditing);
                case ActionType.DeleteExistingTimeEntry:
                    return ShowConfirmationDialog(activity, null, Shared.Resources.DeleteThisTimeEntry, Shared.Resources.Delete, Shared.Resources.Cancel);
                case ActionType.DeleteMultipleExistingTimeEntries:
                    return ShowConfirmationDialog(activity, null, string.Format(Shared.Resources.DeleteMultipleTimeEntries, formatArguments), Shared.Resources.Delete, Shared.Resources.Cancel);
                case ActionType.DiscardFeedback:
                    return ShowConfirmationDialog(activity, null, Shared.Resources.DiscardMessage, Shared.Resources.Discard, Shared.Resources.ContinueEditing);
            }

            throw new ArgumentOutOfRangeException(nameof(type));
        }

        public static void SetQFlags(this Activity activity)
        {
            if (QApis.AreAvailable)
            {
                var uiOptions = SystemUiFlags.LayoutHideNavigation | SystemUiFlags.LayoutStable;
                if (activity.Resources.GetBoolean(Resource.Boolean.is_light_theme))
                {
                    uiOptions |= SystemUiFlags.LightNavigationBar;
                    uiOptions |= SystemUiFlags.LightStatusBar;
                }
                activity.Window.DecorView.SystemUiVisibility = (StatusBarVisibility) (int) uiOptions;
            }
        }
    }
}
