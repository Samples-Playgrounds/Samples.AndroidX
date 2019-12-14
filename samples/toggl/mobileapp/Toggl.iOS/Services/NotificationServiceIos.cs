using Foundation;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core;
using Toggl.Core.Calendar;
using Toggl.Core.UI.Services;
using Toggl.iOS.Extensions;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UserNotifications;
using FoundationResources = Toggl.Shared.Resources;
using Notification = Toggl.Shared.Notification;

namespace Toggl.iOS.Services
{
    public sealed class NotificationServiceIos : PermissionAwareNotificationService
    {
        public const string CalendarEventIdKey = "Id";

        public const string CalendarEventCategory = "CalendarEventCategory";

        public const string OpenAndCreateFromCalendarEvent = "OpenAndStartTimeEntryFromCalendarEvent";
        public const string OpenAndNavigateToCalendar = "OpenAndNavigateToCalendar";
        public const string StartTimeEntryInBackground = "StartTimeEntryInBackground";

        private readonly ITimeService timeService;

        public NotificationServiceIos(IPermissionsChecker permissionsChecker, ITimeService timeService)
            : base(permissionsChecker)
        {
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));

            this.timeService = timeService;

            var openAndCreateFromCalendarEventAction = UNNotificationAction.FromIdentifier(
                OpenAndCreateFromCalendarEvent,
                FoundationResources.OpenAppAndStartAction,
                UNNotificationActionOptions.AuthenticationRequired | UNNotificationActionOptions.Foreground
            );

            var openAndNavigateToCalendarAction = UNNotificationAction.FromIdentifier(
                OpenAndNavigateToCalendar,
                FoundationResources.OpenAppAction,
                UNNotificationActionOptions.AuthenticationRequired | UNNotificationActionOptions.Foreground
            );

            var startTimeEntryInBackgroundAction = UNNotificationAction.FromIdentifier(
                StartTimeEntryInBackground,
                FoundationResources.StartInBackgroundAction,
                UNNotificationActionOptions.AuthenticationRequired
            );

            var calendarEventCategory = UNNotificationCategory.FromIdentifier(
                CalendarEventCategory,
                new UNNotificationAction[] { openAndCreateFromCalendarEventAction, startTimeEntryInBackgroundAction, openAndNavigateToCalendarAction },
                new string[] { },
                UNNotificationCategoryOptions.None
            );

            UNUserNotificationCenter.Current.SetNotificationCategories(new NSSet<UNNotificationCategory>(calendarEventCategory));
        }

        protected override IObservable<Unit> NativeSchedule(IImmutableList<Notification> notifications)
            => Observable.FromAsync(async () =>
                await notifications
                    .Select(notificationRequest)
                    .Select(UNUserNotificationCenter.Current.AddNotificationRequestAsync)
                    .Apply(Task.WhenAll)
            );

        protected override IObservable<Unit> NativeUnscheduleAllNotifications()
        {
            UNUserNotificationCenter.Current.RemoveAllPendingNotificationRequests();

            return Observable.Return(Unit.Default);
        }

        private UNNotificationRequest notificationRequest(Notification notification)
        {
            var identifier = notification.GetIdentifier();

            var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(
                notification.GetTriggerTime(timeService.CurrentDateTime),
                repeats: false
            );

            var content = new UNMutableNotificationContent
            {
                Title = notification.Title,
                Body = notification.Description,
                Sound = UNNotificationSound.Default,
                UserInfo = new NSDictionary(CalendarEventIdKey, notification.Id),
                CategoryIdentifier = CalendarEventCategory
            };

            return UNNotificationRequest.FromIdentifier(identifier, content, trigger);
        }
    }
}
