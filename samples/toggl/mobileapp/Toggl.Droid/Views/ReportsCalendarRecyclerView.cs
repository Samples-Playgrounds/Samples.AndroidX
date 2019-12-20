using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using System;
using AndroidX.RecyclerView.Widget;

namespace Toggl.Droid.Views
{
    [Register("toggl.droid.views.ReportsCalendarRecyclerView")]
    public sealed class ReportsCalendarRecyclerView : RecyclerView
    {
        public ReportsCalendarRecyclerView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public ReportsCalendarRecyclerView(Context context) : base(context)
        {
        }

        public ReportsCalendarRecyclerView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public ReportsCalendarRecyclerView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            var width = MeasureSpec.GetSize(widthMeasureSpec);
            var offset = width % 7;
            var newWidth = width - offset;
            var newWidthMeasureSpec = MeasureSpec.MakeMeasureSpec(newWidth, MeasureSpecMode.Exactly);
            base.OnMeasure(newWidthMeasureSpec, heightMeasureSpec);
        }
    }

    sealed class ReportsCalendarLayoutManager : GridLayoutManager
    {
        public ReportsCalendarLayoutManager(Context context)
            : base(context, 7, LinearLayoutManager.Vertical, false)
        {
        }

        public override bool CanScrollVertically() => false;

        // see the comment in UnpredictiveLinearLayoutManager for rationale
        public override bool SupportsPredictiveItemAnimations() => false;

    }
}
