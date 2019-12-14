using System;
using System.Collections.Generic;

namespace Toggl.Core.Sync.States
{
    public interface IFetchObservables
    {
        IObservable<T> GetSingle<T>();

        IObservable<List<T>> GetList<T>();
    }
}
