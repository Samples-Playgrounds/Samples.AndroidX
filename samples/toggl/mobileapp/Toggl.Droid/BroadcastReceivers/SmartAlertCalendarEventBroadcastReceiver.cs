using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using static Toggl.Droid.Services.NotificationServiceAndroid;

namespace Toggl.Droid.BroadcastReceivers
{
    [BroadcastReceiver]
    public class SmartAlertCalendarEventBroadcastReceiver : BroadcastReceiver
    {
        public static string NotificationId = "NotificationId";
        public static string Notification = "Notification";

        public override void OnReceive(Context context, Intent intent)
        {
            var notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);

            var notification = (Notification)intent.GetParcelableExtra(Notification);
            var notificationId = intent.GetIntExtra(NotificationId, 0);
            
            var sharedPreferences = context.GetSharedPreferences(ScheduledNotificationsSharedPreferencesName, FileCreationMode.Private);
            var savedNotificationIds = sharedPreferences.GetStringSet(ScheduledNotificationsStorageKey, new List<string>()).ToList();

            var removedNotificationIdsCount = savedNotificationIds.RemoveAll(id => id.GetHashCode() == notificationId);

            if (removedNotificationIdsCount > 0) 
                notificationManager.Notify(notificationId, notification);

            sharedPreferences.Edit().PutStringSet(ScheduledNotificationsStorageKey, savedNotificationIds).Commit();
        }
    }
}
