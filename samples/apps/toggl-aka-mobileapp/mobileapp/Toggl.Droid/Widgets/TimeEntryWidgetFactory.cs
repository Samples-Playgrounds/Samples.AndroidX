using Android.OS;
using System;
using Toggl.Droid.Services;

namespace Toggl.Droid.Widgets
{
    public static class TimeEntryWidgetFactory
    {
        public static ITimeEntryWidgetFormFactor Create(WidgetDimensions widgetDimensions)
        {
            var dimensions = widgetDimensions ?? WidgetDimensions.Default;

            if (!WidgetsServiceAndroid.IsLoggedIn)
                return new TimeEntryWidgetNotLoggedInFormFactor(dimensions.ColumnsCount);

            if (dimensions.ColumnsCount == 1)
                return new TimeEntryWidgetButtonOnlyFormFactor();

            return new TimeEntryWidgetDefaultFormFactor();
        }
    }
}
