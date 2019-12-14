using CoreFoundation;
using Toggl.Core.Services;
using UIKit;

namespace Toggl.iOS.Services
{
    public sealed class BackgroundSyncServiceIos : BaseBackgroundSyncService
    {
        public override void EnableBackgroundSync()
        {
            configureInterval(MinimumBackgroundFetchInterval.TotalSeconds);
        }

        public override void DisableBackgroundSync()
        {
            configureInterval(UIApplication.BackgroundFetchIntervalNever);
        }

        private static void configureInterval(double interval)
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
                UIApplication.SharedApplication
                    .SetMinimumBackgroundFetchInterval(interval)
            );
        }
    }
}
