using System;
using System.Collections.Immutable;
using Toggl.Core.DataSources;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Services;
using Toggl.Core.Suggestions;
using Toggl.Shared;

namespace Toggl.Core.UI.Services
{
    public abstract class WidgetsService : IWidgetsService
    {
        private readonly ITogglDataSource dataSource;
        private IDisposable runningTimeEntryDisposable;

        protected WidgetsService(ITogglDataSource dataSource)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            this.dataSource = dataSource;

            runningTimeEntryDisposable = dataSource
                .TimeEntries
                .CurrentlyRunningTimeEntry
                .Subscribe(OnRunningTimeEntryChanged);
        }

        public void Start()
        {
            if (runningTimeEntryDisposable != null)
                return;

            runningTimeEntryDisposable = dataSource
                .TimeEntries
                .CurrentlyRunningTimeEntry
                .Subscribe(OnRunningTimeEntryChanged);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!isDisposing)
                return;

            runningTimeEntryDisposable?.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected abstract void OnRunningTimeEntryChanged(IThreadSafeTimeEntry timeEntry);
        public abstract void OnSuggestionsUpdated(IImmutableList<Suggestion> suggestions);
    }
}
