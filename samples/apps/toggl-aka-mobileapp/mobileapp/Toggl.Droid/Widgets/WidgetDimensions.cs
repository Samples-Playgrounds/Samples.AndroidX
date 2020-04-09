using Android.Appwidget;
using Android.Content;
using Android.OS;

namespace Toggl.Droid.Widgets
{
    public sealed class WidgetDimensions
    {
        private static readonly int defaultMinWidth = 250;
        private static readonly int defaultMinHeight = 40;

        public int MinWidth { get; private set; }
        public int MinHeight { get; private set; }
        public int ColumnsCount { get; private set; }

        private WidgetDimensions()
        {
            MinWidth = defaultMinWidth;
            MinHeight = defaultMinHeight;
            ColumnsCount = getColumnsCount(MinWidth);
        }

        private WidgetDimensions(int minWidth, int minHeight)
        {
            MinWidth = minWidth;
            MinHeight = minHeight;

            ColumnsCount = getColumnsCount(MinWidth);
        }

        public static WidgetDimensions Default
            => new WidgetDimensions();

        public static WidgetDimensions FromBundle(Bundle bundle)
        {
            var minWidth = bundle.GetInt(AppWidgetManager.OptionAppwidgetMinWidth);
            var minHeight = bundle.GetInt(AppWidgetManager.OptionAppwidgetMinHeight);

            return new WidgetDimensions(minWidth, minHeight);
        }

        /// <summary>
        /// Calculates the number of columns used by the widget based on the given width
        /// </summary>
        /// <remarks>
        /// Magic numbers in this method come from https://developer.android.com/guide/practices/ui_guidelines/widget_design.html#anatomy_determining_size
        /// </remarks>
        private static int getColumnsCount(int width)
            => (width + 30) / 70;
    }
}
