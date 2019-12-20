using Android.Graphics;

namespace Toggl.Droid.Views.EditDuration.Shapes
{
    public sealed class Wheel
    {
        private readonly Paint paint = new Paint(PaintFlags.AntiAlias);
        private readonly RectF bounds;

        private bool hidden;

        public Wheel(RectF bounds, float strokeWidth, Color fillColor)
        {
            this.bounds = bounds;
            paint.Color = fillColor;
            paint.SetStyle(Paint.Style.Stroke);
            paint.StrokeWidth = strokeWidth;
            hidden = false;
        }

        public void OnDraw(Canvas canvas)
        {
            if (hidden) return;

            canvas.DrawArc(bounds, 0f, 360f, false, paint);
        }

        public void SetFillColor(Color color)
        {
            paint.Color = color;
        }

        public void SetHidden(bool hidden)
        {
            this.hidden = hidden;
        }
    }
}
