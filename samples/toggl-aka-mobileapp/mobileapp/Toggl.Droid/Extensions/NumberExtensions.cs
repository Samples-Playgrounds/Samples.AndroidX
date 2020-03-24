using Android.Content;
using Android.Util;

namespace Toggl.Droid.Extensions
{
    public static class NumberExtensions
    {
        public static int DpToPixels(this int self, Context context)
            => ((float)self).DpToPixels(context);

        public static int DpToPixels(this float self, Context context)
            => (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, self, context.Resources.DisplayMetrics);

        public static int SpToPixels(this int self, Context context)
            => ((float)self).SpToPixels(context);

        public static int SpToPixels(this float self, Context context)
            => (int)TypedValue.ApplyDimension(ComplexUnitType.Sp, self, context.Resources.DisplayMetrics);
    }
}
