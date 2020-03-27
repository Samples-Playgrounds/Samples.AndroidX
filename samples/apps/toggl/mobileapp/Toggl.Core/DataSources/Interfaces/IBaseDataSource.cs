using System;
using System.Collections.Generic;
using Toggl.Core.Models.Interfaces;
using Toggl.Storage;

namespace Toggl.Core.DataSources.Interfaces
{
    public interface IBaseDataSource<T>
        where T : IThreadSafeModel
    {
        IObservable<T> Create(T entity);

        IObservable<T> Update(T entity);

        IObservable<IEnumerable<IConflictResolutionResult<T>>> OverwriteIfOriginalDidNotChange(T original, T entity);
    }
}
