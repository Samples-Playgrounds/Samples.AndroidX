using Foundation;
using System;
using Toggl.Core.UI.Services;
using Toggl.iOS.Helper;
using UserNotifications;

namespace Toggl.iOS.Services
{
    [Preserve(AllMembers = true)]
    public sealed class PermissionsCheckerIos : IPermissionsChecker
    {
        private readonly UNAuthorizationOptions options =
            UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;

        public IObservable<bool> CalendarPermissionGranted
            => PermissionsHelper.CalendarPermissionGranted;

        public IObservable<bool> NotificationPermissionGranted
            => PermissionsHelper.NotificationPermissionGranted;
    }
}
