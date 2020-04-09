using System;
using Toggl.iOS.Helper;

namespace Toggl.iOS.ViewControllers
{
    public partial class ReactiveViewController<TViewModel>
    {
        public IObservable<bool> RequestCalendarAuthorization(bool force = false)
            => PermissionsHelper.RequestCalendarAuthorization(force);

        public IObservable<bool> RequestNotificationAuthorization(bool force = false)
            => PermissionsHelper.RequestNotificationAuthorization(force);

        public void OpenAppSettings()
        {
            PermissionsHelper.OpenAppSettings();
        }
    }
}
