using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Android.Content;
using Android.Content.PM;
using AndroidX.Core.Content;
using Toggl.Core.UI.Views;
using Toggl.Droid.Extensions;
using Toggl.Droid.Helper;

namespace Toggl.Droid.Fragments.Calendar
{
    public partial class CalendarDayViewPageFragment : IPermissionRequesterComponent
    {
        public Subject<bool> CalendarAuthorizationSubject { get; set; }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            this.ProcessRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public Permission CheckPermission(string permission)
            => ContextCompat.CheckSelfPermission(Activity, permission);

        public void StartActivityIntent(Intent intent)
            => StartActivity(intent);

        public IObservable<bool> RequestCalendarAuthorization(bool force = false)
            => this.ProcessCalendarAuthorizationRequest(force);

        public IObservable<bool> RequestNotificationAuthorization(bool force = false)
            => Observable.Return(true);

        public void OpenAppSettings()
            => this.FireAppSettingsIntent();
        
        public IObservable<string> GetGoogleToken()
        {
            if (!(Activity is IGoogleTokenProvider tokenProvider))
                throw new InvalidOperationException();

            return tokenProvider.GetGoogleToken();
        }

        public void Close()
        {
        }
    }
}