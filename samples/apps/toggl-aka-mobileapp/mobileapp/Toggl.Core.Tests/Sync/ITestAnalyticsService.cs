using Toggl.Core.Analytics;

namespace Toggl.Core.Tests.Sync
{
    public interface ITestAnalyticsService : IAnalyticsService
    {
        IAnalyticsEvent<string> TestEvent { get; }
    }
}
