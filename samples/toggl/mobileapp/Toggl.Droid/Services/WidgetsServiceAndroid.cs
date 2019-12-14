using Android.App;
using Android.Appwidget;
using Android.Content;
using System.Collections.Immutable;
using Toggl.Core.DataSources;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Suggestions;
using Toggl.Core.UI.Services;
using Toggl.Droid.Extensions;
using Toggl.Droid.Helper;
using Toggl.Droid.Widgets;

namespace Toggl.Droid.Services
{
    public sealed class WidgetsServiceAndroid : WidgetsService
    {
        public WidgetsServiceAndroid(ITogglDataSource dataSource) : base(dataSource)
        {
        }

        protected override void OnRunningTimeEntryChanged(IThreadSafeTimeEntry timeEntry)
        {
            TimeEntryWidgetInfo.Save(timeEntry);
            AppWidgetProviderUtils.UpdateAllInstances<TimeEntryWidget>();
        }

        public override void OnSuggestionsUpdated(IImmutableList<Suggestion> suggestions)
        {
            WidgetSuggestionItem.SaveSuggestions(suggestions);
            AppWidgetProviderUtils.UpdateAllInstances<SuggestionsWidget>();
        }

        public static bool IsLoggedIn =>
            AndroidDependencyContainer.Instance.PrivateSharedStorageService.GetApiToken() != null;
    }
}
