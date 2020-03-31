using Android.Content;
using Android.Widget;
using System;

namespace Toggl.Droid.Widgets
{
    public interface ITimeEntryWidgetFormFactor
    {
        RemoteViews Setup(Context context, TimeEntryWidgetInfo widgetInfo);
    }
}
