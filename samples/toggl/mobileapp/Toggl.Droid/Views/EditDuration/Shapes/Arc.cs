using Android.Graphics;
using static Toggl.Shared.Math;
using Math = Java.Lang.Math;

namespace Toggl.Droid.Views.EditDuration.Shapes
{
    public class Arc
    {
        private readonly Paint paint = new Paint(PaintFlags.AntiAlias);
        private readonly RectF bounds;

        private float startAngle;
        private float endAngle;
        private float endStroke;

        public Arc(RectF bounds, float strokeWidth, Color fillColor)
        {
            this.bounds = bounds;
            paint.Color = fillColor;
            paint.SetStyle(Paint.Style.Stroke);
            paint.StrokeWidth = strokeWidth;
        }

        public void SetFillColor(Color color)
        {
            paint.Color = color;
        }

        public void OnDraw(Canvas canvas)
        {
            var startAngleInDegrees = Math.ToDegrees(startAngle);
            var endStrokeInDegrees = Math.ToDegrees(endStroke);
            canvas.DrawArc(bounds, (float)startAngleInDegrees, (float)endStrokeInDegrees, false, paint);
        }

        public void Update(double startTimeAngle, double endTimeAngle)
        {
            startAngle = (float)startTimeAngle;
            endAngle = (float)endTimeAngle;
            var diffAngle = endAngle - startAngle + (endAngle < startAngle ? FullCircle : 0);
            endStroke = (float)diffAngle;
        }
    }
}
