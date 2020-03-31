using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Suggestions;
using Toggl.Core.UI.Services;
using Toggl.iOS.Extensions;
using Toggl.iOS.Shared;
using Toggl.Shared;

namespace Toggl.iOS.Services
{
    public sealed class WidgetsServiceIos : WidgetsService
    {
        private IDisposable durationFormatDisposable;

        public WidgetsServiceIos(ITogglDataSource dataSource) : base(dataSource)
        {
            durationFormatDisposable = dataSource
                .Preferences
                .Current
                .Select(preferences => preferences.DurationFormat)
                .Subscribe(onDurationFormat);
        }

        protected override void OnRunningTimeEntryChanged(IThreadSafeTimeEntry timeEntry)
        {
            if (timeEntry == null)
            {
                SharedStorage.Instance.SetRunningTimeEntry(null);
                return;
            }

            SharedStorage.Instance.SetRunningTimeEntry(
                timeEntry,
                timeEntry.Project?.Name ?? "",
                timeEntry.Project?.Color ?? "",
                timeEntry.Task?.Name ?? "",
                timeEntry.Project?.Client?.Name ?? "");
        }

        public override void OnSuggestionsUpdated(IImmutableList<Suggestion> suggestions)
        {
            var sharedSuggestions = suggestions.Select(SuggestionExtensions.ToSharedSuggestion).ToList();
            SharedStorage.Instance.SetCurrentSuggestions(sharedSuggestions);
        }

        private void onDurationFormat(DurationFormat durationFormat)
        {
            SharedStorage.Instance.SetDurationFormat((int) durationFormat);
        }
    }
}
