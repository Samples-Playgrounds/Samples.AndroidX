using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using AndroidX.ViewPager.Widget;

namespace Toggl.Droid.Fragments
{
    [Register("toggl.droid.views.LockableViewPager")]
    public class LockableViewPager : ViewPager
    {
        public bool IsLocked { get; set; }

        protected LockableViewPager(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public LockableViewPager(Context context) : base(context)
        {
        }

        public LockableViewPager(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (IsLocked)
                return false;

            return base.OnTouchEvent(e);
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            if (IsLocked)
                return false;

            return base.OnInterceptTouchEvent(ev);
        }
    }
}