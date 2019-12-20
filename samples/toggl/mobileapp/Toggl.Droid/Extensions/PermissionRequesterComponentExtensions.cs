using Android;
using Android.Content;
using Android.Content.PM;
using Android.Provider;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AndroidX.Core.Content;
using Toggl.Droid.Helper;

namespace Toggl.Droid.Extensions
{
    public static class PermissionRequesterComponentExtensions
    {
        private const int calendarAuthCode = 500;

        public static void ProcessRequestPermissionsResult(this IPermissionRequesterComponent permissionRequester, int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (requestCode != calendarAuthCode)
            {
                permissionRequester.CalendarAuthorizationSubject?.OnNext(false);
                permissionRequester.CalendarAuthorizationSubject?.OnCompleted();
                permissionRequester.CalendarAuthorizationSubject = null;
            }

            var permissionWasGranted = grantResults.Any() && grantResults.First() == Permission.Granted;
            permissionRequester.CalendarAuthorizationSubject?.OnNext(permissionWasGranted);
            permissionRequester.CalendarAuthorizationSubject?.OnCompleted();
            permissionRequester.CalendarAuthorizationSubject = null;
        }

        public static IObservable<bool> ProcessCalendarAuthorizationRequest(this IPermissionRequesterComponent permissionRequester, bool force = false)
            => Observable.Defer(() =>
            {
                if (permissionRequester.checkPermissions(Manifest.Permission.ReadCalendar))
                    return Observable.Return(true);

                if (permissionRequester.CalendarAuthorizationSubject != null)
                    return permissionRequester.CalendarAuthorizationSubject.AsObservable();

                permissionRequester.CalendarAuthorizationSubject = new Subject<bool>();
                permissionRequester.RequestPermissions(new[] { Manifest.Permission.ReadCalendar }, calendarAuthCode);

                return permissionRequester.CalendarAuthorizationSubject.AsObservable();
            });

        public static void FireAppSettingsIntent(this IPermissionRequesterComponent permissionRequester)
        {
            var settingsIntent = new Intent();
            settingsIntent.SetAction(Settings.ActionApplicationDetailsSettings);
            settingsIntent.AddCategory(Intent.CategoryDefault);
            settingsIntent.SetData(Android.Net.Uri.Parse("package:com.toggl.giskard"));
            settingsIntent.AddFlags(ActivityFlags.NewTask);
            settingsIntent.AddFlags(ActivityFlags.NoHistory);
            settingsIntent.AddFlags(ActivityFlags.ExcludeFromRecents);

            permissionRequester.StartActivityIntent(settingsIntent);
        }

        private static bool checkPermissions(this IPermissionRequesterComponent permissionRequester, params string[] permissionsToCheck)
        {
            foreach (var permission in permissionsToCheck)
            {
                if (MarshmallowApis.AreAvailable)
                {
                    if (permissionRequester.CheckPermission(permission) != Permission.Granted)
                        return false;
                }
                else
                {
                    if (permissionRequester.CheckPermission(permission) != PermissionChecker.PermissionGranted)
                        return false;
                }
            }

            return true;
        }
    }
}