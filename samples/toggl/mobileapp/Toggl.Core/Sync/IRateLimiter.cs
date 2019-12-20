using System;
using System.Reactive;

namespace Toggl.Core.Sync
{
    public interface IRateLimiter
    {
        IObservable<Unit> WaitForFreeSlot();
    }
}
