using WatchConnectivity;

namespace Toggl.iOS.Services
{
    public sealed class WatchService : WCSessionDelegate
    {
        public void TryLogWatchConnectivity()
        {
            if (WCSession.IsSupported) {
                var session = WCSession.DefaultSession;
                session.Delegate = this;
                session.ActivateSession();
                IosDependencyContainer.Instance.AnalyticsService.WatchPaired.Track(session.Paired);
            }
        }
    }
}
