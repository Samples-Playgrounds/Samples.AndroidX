using System.Reactive.Concurrency;

namespace Toggl.Shared
{
    public interface ISchedulerProvider
    {
        IScheduler MainScheduler { get; }
        IScheduler DefaultScheduler { get; }
        IScheduler BackgroundScheduler { get; }
    }
}
