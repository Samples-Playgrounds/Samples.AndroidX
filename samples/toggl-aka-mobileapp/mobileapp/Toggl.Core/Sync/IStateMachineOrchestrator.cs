using System;

namespace Toggl.Core.Sync
{
    public interface IStateMachineOrchestrator
    {
        SyncState State { get; }
        IObservable<SyncState> StateObservable { get; }
        IObservable<SyncResult> SyncCompleteObservable { get; }

        void Start(SyncState state);
        void Freeze();
    }
}
