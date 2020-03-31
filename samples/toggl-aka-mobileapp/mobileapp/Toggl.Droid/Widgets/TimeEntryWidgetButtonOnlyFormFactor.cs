using Android.Content;
using Android.Widget;
using System;
using Toggl.Droid.Extensions;

namespace Toggl.Droid.Widgets
{
    public sealed class TimeEntryWidgetButtonOnlyFormFactor : TimeEntryWidgetStartStopButtonFormFactor
    {
        public override RemoteViews Setup(Context context, TimeEntryWidgetInfo widgetInfo)
        {
            var view = new RemoteViews(context.PackageName, Resource.Layout.TimeEntryWidgetSmall);

            SetupActionsForStartAndStopButtons(context, view);

            view.SetViewVisibility(Resource.Id.StartButton, (!widgetInfo.IsRunning).ToVisibility());
            view.SetViewVisibility(Resource.Id.StopButton, widgetInfo.IsRunning.ToVisibility());

            return view;
        }
    }
}
