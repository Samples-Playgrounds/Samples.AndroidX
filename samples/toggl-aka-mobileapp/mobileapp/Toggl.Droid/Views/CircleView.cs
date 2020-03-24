using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using System;

namespace Toggl.Droid.Views
{
    [Register("toggl.droid.views.CircleView")]
    public sealed class CircleView : View
    {
        private readonly Paint paint;

        public void SetCircleColor(Color color)
        {
            paint.Color = color;
            Invalidate();
        }

        public CircleView(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public CircleView(Context context, IAttributeSet attrs)
            : this(context, attrs, 0)
        {
        }

        public CircleView(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            paint = new Paint { Flags = PaintFlags.AntiAlias };
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, widthMeasureSpec);
        }

        public override void Draw(Canvas canvas)
        {
            var center = Width / 2;
            canvas.DrawCircle(center, center, center, paint);
        }
    }
}
