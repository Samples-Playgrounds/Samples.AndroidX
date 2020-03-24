using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Widget;
using Toggl.Droid.Extensions;
using Toggl.Droid.Helper;
using Toggl.Droid.Services;
using Toggl.Shared;
using static Android.App.PendingIntentFlags;
using static Android.Content.ActivityFlags;
using static Toggl.Droid.Widgets.WidgetsConstants;

namespace Toggl.Droid.Widgets
{
    public class SuggestionsWidgetListFormFactor : ISuggestionsWidgetFormFactor
    {
        public bool ContainsListView { get; } = true;

        public RemoteViews Setup(Context context, int widgetId)
        {
            var view = new RemoteViews(context.PackageName, Resource.Layout.SuggestionsWidget);

            var intent = new Intent(context, JavaUtils.ToClass<SuggestionsWidgetService>());
            intent.PutExtra(AppWidgetManager.ExtraAppwidgetId, widgetId);

            view.SetRemoteAdapter(Resource.Id.SuggestionsList, intent);
            view.SetEmptyView(Resource.Id.SuggestionsList, Resource.Id.NoData);

            var tapIntent = new Intent(context, JavaUtils.ToClass<WidgetsForegroundService>());
            tapIntent.SetAction(SuggestionTapped);
            tapIntent.PutExtra(AppWidgetManager.ExtraAppwidgetId, widgetId);
            var tapPendingIntent = context.SafeGetForegroundService(0, tapIntent, UpdateCurrent);
            view.SetPendingIntentTemplate(Resource.Id.SuggestionsList, tapPendingIntent);

            view.SetTextViewText(Resource.Id.Title, Resources.WorkingOnThese);
            view.SetTextViewText(Resource.Id.NoData, Resources.NoSuggestionsAvailable);
            view.SetTextViewText(Resource.Id.ShowAllTimeEntriesLabel, Resources.ShowAllTimEntries);

            var openAppIntent = new Intent(context, typeof(SplashScreen)).SetFlags(TaskOnHome);
            var openAppPendingIntent = PendingIntent.GetActivity(context, 0, openAppIntent, UpdateCurrent);
            view.SetOnClickPendingIntent(Resource.Id.ShowAllTimeEntriesLabel, openAppPendingIntent);

            return view;
        }
    }
}
