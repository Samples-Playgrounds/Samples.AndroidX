using System;

namespace Toggl.Core.Sync
{
    public interface ILeakyBucket
    {
        bool TryClaimFreeSlot(out TimeSpan timeToFreeSlot);
        bool TryClaimFreeSlots(int numberOfSlots, out TimeSpan timeToFreeSlot);
    }
}
