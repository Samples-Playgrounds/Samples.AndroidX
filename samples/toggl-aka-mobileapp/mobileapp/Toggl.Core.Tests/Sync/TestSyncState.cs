using System;
using Toggl.Core.Sync;

namespace Toggl.Core.Tests.Sync
{
    public class TestSyncState : ISyncState
    {
        private readonly Func<IObservable<ITransition>> start;

        public TestSyncState(Func<IObservable<ITransition>> start)
        {
            this.start = start;
        }

        public IObservable<ITransition> Start() => start();
    }

    public class TestSyncState<T> : ISyncState<T>
    {
        private readonly Func<T, IObservable<ITransition>> start;

        public TestSyncState(Func<T, IObservable<ITransition>> start)
        {
            this.start = start;
        }

        public IObservable<ITransition> Start(T parameter) => start(parameter);
    }
}
