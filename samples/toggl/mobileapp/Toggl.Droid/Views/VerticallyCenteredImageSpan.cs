using System;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Text.Style;
using Java.Lang;
using WeakReference = Java.Lang.Ref.WeakReference;

namespace Toggl.Droid.Views
{
    public class VerticallyCenteredImageSpan : ImageSpan
    {
        private WeakReference drawableWeakRef;
        
        protected VerticallyCenteredImageSpan(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public VerticallyCenteredImageSpan(Context context, int resourceId) : base(context, resourceId)
        {
        }

        public override void Draw(Canvas canvas, ICharSequence text, int start, int end, float x, int top, int y, int bottom, Paint paint)
        {
            canvas.Save();
            
            var cachedDrawable = getCachedDrawable();
            var transY = bottom - cachedDrawable.Bounds.Bottom - (paint.GetFontMetricsInt().Descent - (int)(paint.GetFontMetrics().Ascent / 2f));
            canvas.Translate(x, (-paint.GetFontMetrics().Ascent) / 2f + transY / 2f);
            cachedDrawable.Draw(canvas);
            
            canvas.Restore();
        }

        private Drawable getCachedDrawable()
        {
            Drawable drawable = (Drawable)drawableWeakRef?.Get();
            if (drawable == null)
            {
                drawable = Drawable;
                drawableWeakRef = new WeakReference(drawable);
            }

            return drawable;
        }
    }
}