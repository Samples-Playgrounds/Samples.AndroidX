using System.Reactive.Concurrency;
using Toggl.Shared;

namespace Toggl.iOS
{
    public class IOSSchedulerProvider : ISchedulerProvider
    {
        public IScheduler MainScheduler { get; }
        public IScheduler DefaultScheduler { get; }
        public IScheduler BackgroundScheduler { get; }

        public IOSSchedulerProvider()
        {
            MainScheduler = new NSRunloopScheduler();
            DefaultScheduler = Scheduler.Default;
            BackgroundScheduler = NewThreadScheduler.Default;
        }
    }
}
