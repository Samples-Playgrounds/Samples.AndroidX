using System;
using Toggl.Core.Models.Interfaces;
using Toggl.Storage;

namespace Toggl.Core.DataSources.Interfaces
{
    public interface ISingletonDataSource<T> : IBaseDataSource<T>
        where T : IThreadSafeModel
    {
        IObservable<T> Current { get; }

        IObservable<T> Get();

        IObservable<IConflictResolutionResult<T>> UpdateWithConflictResolution(T entity);
    }
}
