using Android.Appwidget;
using Android.Content;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using Toggl.Shared;
using Color = Android.Graphics.Color;
using Toggl.Droid.Extensions;
using Android.App;
using Toggl.Droid.Services;
using static Toggl.Droid.Widgets.WidgetsConstants;
using static Android.App.PendingIntentFlags;
using static Android.Content.ActivityFlags;
using Toggl.Droid.Services;

namespace Toggl.Droid.Widgets
{
    public sealed class SuggestionsWidgetFactory
    {
        public static ISuggestionsWidgetFormFactor Create(WidgetDimensions dimensions)
        {
            if (!WidgetsServiceAndroid.IsLoggedIn)
                return new SuggestionsWidgetNotLoggedInFormFactor(dimensions.ColumnsCount);

            return new SuggestionsWidgetListFormFactor();
        }
    }
}
