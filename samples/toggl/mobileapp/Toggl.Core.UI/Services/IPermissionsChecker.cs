using System;

namespace Toggl.Core.UI.Services
{
    public interface IPermissionsChecker
    {
        IObservable<bool> CalendarPermissionGranted { get; }

        IObservable<bool> NotificationPermissionGranted { get; }
    }
}
