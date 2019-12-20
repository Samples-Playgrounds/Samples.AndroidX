using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using Toggl.Droid.Extensions;
using Toggl.Shared;
using static Android.App.PendingIntentFlags;
using static Android.Content.ActivityFlags;
using static Toggl.Droid.Widgets.WidgetsConstants;
using Color = Android.Graphics.Color;

namespace Toggl.Droid.Widgets
{
    public sealed class SuggestionsWidgetNotLoggedInFormFactor : WidgetNotLoggedInFormFactor, ISuggestionsWidgetFormFactor
    {
        public bool ContainsListView { get; } = false;
        protected override string LabelText { get; } = Resources.LoginToShowSuggestions;

        public SuggestionsWidgetNotLoggedInFormFactor(int columnCount)
            : base(columnCount) { }

        public RemoteViews Setup(Context context, int widgetId)
            => Setup(context);
    }
}
