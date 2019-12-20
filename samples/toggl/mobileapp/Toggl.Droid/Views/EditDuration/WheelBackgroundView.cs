using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using System;
using Toggl.Droid.Extensions;
using Toggl.Droid.Views.EditDuration.Shapes;

namespace Toggl.Droid.Views.EditDuration
{
    [Register("toggl.droid.views.WheelBackgroundView")]
    public class WheelBackgroundView : View
    {
        private readonly Color wheelColor = Color.ParseColor("#f3f3f3");
        private readonly Color textColor = Color.ParseColor("#959595");

        private PointF center;
        private RectF bounds;

        private float radius;
        private float arcWidth;
        private float capWidth;
        private float textRadius;

        private Wheel wheel;
        private ClockDial clockDial;

        #region Constructors

        protected WheelBackgroundView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public WheelBackgroundView(Context context) : base(context)
        {
            init();
        }

        public WheelBackgroundView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            init();
        }

        public WheelBackgroundView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            init();
        }

        public WheelBackgroundView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
            init();
        }

        private void init()
        {
            arcWidth = 8.DpToPixels(Context);
            capWidth = 28.DpToPixels(Context);
        }

        #endregion

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            base.OnLayout(changed, left, top, right, bottom);
            radius = Width * 0.5f;
            center = new PointF(radius, radius);
            bounds = new RectF(capWidth, capWidth, Width - capWidth, Width - capWidth);
            textRadius = radius - capWidth * 2 - 2.DpToPixels(Context);
            setupDrawingDelegates();
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            wheel.OnDraw(canvas);
            clockDial.OnDraw(canvas);
        }

        private void setupDrawingDelegates()
        {
            wheel = new Wheel(bounds, arcWidth, wheelColor);
            clockDial = new ClockDial(center, 15.SpToPixels(Context), textColor, textRadius);
        }
    }
}
