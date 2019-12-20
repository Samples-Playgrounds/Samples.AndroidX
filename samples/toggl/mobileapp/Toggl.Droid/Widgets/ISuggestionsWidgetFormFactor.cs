using Android.Content;
using Android.Widget;
using System;

namespace Toggl.Droid.Widgets
{
    public interface ISuggestionsWidgetFormFactor
    {
        RemoteViews Setup(Context context, int widgetId);
        bool ContainsListView { get; }
    }
}
