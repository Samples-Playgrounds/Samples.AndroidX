using System;
using System.Reactive;
using Toggl.Core.Models.Interfaces;
using Toggl.Storage;

namespace Toggl.Core.DataSources.Interfaces
{
    public interface IObservableDataSource<TThreadsafe, out TDatabase>
        : IDataSource<TThreadsafe, TDatabase>
        where TDatabase : IDatabaseSyncable
        where TThreadsafe : IThreadSafeModel, TDatabase
    {
        IObservable<Unit> ItemsChanged { get; }
    }
}
