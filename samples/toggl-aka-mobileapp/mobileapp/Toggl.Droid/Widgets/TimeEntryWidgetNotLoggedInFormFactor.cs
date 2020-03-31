using System;
using Android.Content;
using Android.Widget;
using Toggl.Shared;

namespace Toggl.Droid.Widgets
{
    public sealed class TimeEntryWidgetNotLoggedInFormFactor : WidgetNotLoggedInFormFactor, ITimeEntryWidgetFormFactor
    {
        protected override string LabelText { get; } = Resources.LoginToTrack;

        public TimeEntryWidgetNotLoggedInFormFactor(int columnCount)
            : base(columnCount) { }

        public RemoteViews Setup(Context context, TimeEntryWidgetInfo widgetInfo)
            => Setup(context);
    }
}
