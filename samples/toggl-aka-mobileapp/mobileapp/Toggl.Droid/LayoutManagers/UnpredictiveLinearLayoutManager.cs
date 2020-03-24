using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using AndroidX.RecyclerView.Widget;

namespace Toggl.Droid.LayoutManagers
{
    public class UnpredictiveLinearLayoutManager : LinearLayoutManager
    {
        protected UnpredictiveLinearLayoutManager(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public UnpredictiveLinearLayoutManager(Context context) : base(context)
        {
        }

        public UnpredictiveLinearLayoutManager(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        public UnpredictiveLinearLayoutManager(Context context, int orientation, bool reverseLayout) : base(context, orientation, reverseLayout)
        {
        }

        // Making the following method return false fixes
        // java.lang.IndexOutOfBoundsException: Inconsistency detected.
        // in RecyclerView's tryGetViewHolderForPositionByDeadline
        public override bool SupportsPredictiveItemAnimations() => false;
    }
}