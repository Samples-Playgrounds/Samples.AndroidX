using Microsoft.Reactive.Testing;
using System.Reactive.Concurrency;
using Toggl.Shared;

namespace Toggl.Core.Tests.UI
{
    public sealed class TestSchedulerProvider : ISchedulerProvider
    {
        public TestScheduler TestScheduler { get; }

        public TestSchedulerProvider()
        {
            TestScheduler = new TestScheduler();
        }

        public IScheduler MainScheduler => TestScheduler;
        public IScheduler DefaultScheduler => TestScheduler;
        public IScheduler BackgroundScheduler => TestScheduler;
    }
}
