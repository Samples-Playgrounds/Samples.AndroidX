using Android.Views;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.RecyclerView.Widget;

namespace Toggl.Droid.Extensions
{
    public static class LayoutParamsExtensions
    {
        public static ViewGroup.MarginLayoutParams WithMargins(
            this ViewGroup.MarginLayoutParams self,
            int? left = null,
            int? top = null,
            int? right = null,
            int? bottom = null)
        {
            var actualTop = top ?? self.TopMargin;
            var actualLeft = left ?? self.LeftMargin;
            var actualRight = right ?? self.RightMargin;
            var actualBottom = bottom ?? self.BottomMargin;

            switch (self)
            {
                case LinearLayout.LayoutParams _:
                    var newLinearLayoutParams = new LinearLayout.LayoutParams(self);
                    newLinearLayoutParams.SetMargins(actualLeft, actualTop, actualRight, actualBottom);
                    return newLinearLayoutParams;

                case RelativeLayout.LayoutParams _:
                    var newRelativeLayoutParams = new RelativeLayout.LayoutParams(self);
                    newRelativeLayoutParams.SetMargins(actualLeft, actualTop, actualRight, actualBottom);
                    return newRelativeLayoutParams;

                case FrameLayout.LayoutParams _:
                    var newFrameLayoutParams = new FrameLayout.LayoutParams(self);
                    newFrameLayoutParams.SetMargins(actualLeft, actualTop, actualRight, actualBottom);
                    return newFrameLayoutParams;

                case RecyclerView.LayoutParams _:
                    var newRecyclerParams = new FrameLayout.LayoutParams(self);
                    newRecyclerParams.SetMargins(actualLeft, actualTop, actualRight, actualBottom);
                    return newRecyclerParams;

                case ConstraintLayout.LayoutParams _:
                    var newConstraintParams = new ConstraintLayout.LayoutParams(self);
                    newConstraintParams.SetMargins(actualLeft, actualTop, actualRight, actualBottom);
                    return newConstraintParams;
            }

            return null;
        }
    }
}
