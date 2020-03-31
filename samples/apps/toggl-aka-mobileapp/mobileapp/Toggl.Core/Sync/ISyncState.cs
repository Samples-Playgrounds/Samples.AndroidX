using System;

namespace Toggl.Core.Sync
{
    public interface ISyncState
    {
        IObservable<ITransition> Start();
    }

    public interface ISyncState<in T>
    {
        IObservable<ITransition> Start(T parameterNamedSensiblyByImplementation);
    }
}
