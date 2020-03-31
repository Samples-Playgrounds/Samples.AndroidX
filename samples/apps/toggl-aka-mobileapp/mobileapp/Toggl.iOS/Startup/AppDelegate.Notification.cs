using Foundation;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.Extensions;
using Toggl.Core.Interactors;
using Toggl.Core.Models.Interfaces;
using Toggl.iOS.Services;
using Toggl.Shared;
using UserNotifications;

namespace Toggl.iOS
{
    public partial class AppDelegate
    {
        [Export("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:")]
        public void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            var eventId = response.Notification.Request.Content.UserInfo[NotificationServiceIos.CalendarEventIdKey] as NSString;

            if (response.IsCustomAction)
            {
                switch (response.ActionIdentifier.ToString())
                {
                    case NotificationServiceIos.OpenAndCreateFromCalendarEvent:
                        openAndStartTimeEntryFromCalendarEvent(eventId.ToString(), completionHandler);
                        break;
                    case NotificationServiceIos.OpenAndNavigateToCalendar:
                        openAndNavigateToCalendar(completionHandler);
                        break;
                    case NotificationServiceIos.StartTimeEntryInBackground:
                        startTimeEntryInBackground(eventId.ToString(), completionHandler);
                        break;
                }
            }
            else if (response.IsDefaultAction)
            {
                openAndStartTimeEntryFromCalendarEvent(eventId.ToString(), completionHandler);
            }
        }

        private void openAndStartTimeEntryFromCalendarEvent(string eventId, Action completionHandler)
        {
            completionHandler();
            var url = ApplicationUrls.Calendar.ForId(eventId);
            handleDeeplink(new Uri(url));
        }

        private void openAndNavigateToCalendar(Action completionHandler)
        {
            completionHandler();
            var url = ApplicationUrls.Calendar.Default;
            handleDeeplink(new Uri(url));
        }

        private void startTimeEntryInBackground(string eventId, Action completionHandler)
        {
            var interactorFactory = IosDependencyContainer.Instance.InteractorFactory;

            Task.Run(async () =>
            {
                var calendarItem = await interactorFactory.GetCalendarItemWithId(eventId).Execute();

                var now = IosDependencyContainer.Instance.TimeService.CurrentDateTime;
                var workspace = await interactorFactory.GetDefaultWorkspace()
                    .TrackException<InvalidOperationException, IThreadSafeWorkspace>("AppDelegate.startTimeEntryInBackground")
                    .Execute();

                var prototype = calendarItem.Description.AsTimeEntryPrototype(now, workspace.Id);
                await interactorFactory.CreateTimeEntry(prototype, TimeEntryStartOrigin.CalendarNotification).Execute();
                completionHandler();
            });
        }


    }
}
