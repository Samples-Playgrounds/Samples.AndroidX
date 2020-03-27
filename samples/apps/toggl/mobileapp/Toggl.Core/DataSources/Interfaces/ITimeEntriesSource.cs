using System;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models.Interfaces;
using Toggl.Storage.Models;

namespace Toggl.Core.DataSources
{
    public interface ITimeEntriesSource
        : IObservableDataSource<IThreadSafeTimeEntry, IDatabaseTimeEntry>
    {
        IObservable<IThreadSafeTimeEntry> TimeEntryStarted { get; }
        IObservable<IThreadSafeTimeEntry> TimeEntryStopped { get; }
        IObservable<IThreadSafeTimeEntry> TimeEntryContinued { get; }
        IObservable<IThreadSafeTimeEntry> SuggestionStarted { get; }

        IObservable<IThreadSafeTimeEntry> CurrentlyRunningTimeEntry { get; }

        IObservable<bool> IsEmpty { get; }
    }
}
