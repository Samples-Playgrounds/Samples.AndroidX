using Android.Graphics;
using Toggl.Droid.Extensions;
using static Toggl.Shared.Math;

namespace Toggl.Droid.Views.EditDuration.Shapes
{
    public class ClockDial
    {
        private const float angleOffsetCorrection = (float)FullCircle / 4f;
        private const char numberPaddingChar = '0';
        private const int digitsCount = 2;
        private readonly PointF dialCenter;
        private readonly float textRadius;
        private Rect textBounds = new Rect();
        private PointF textCenter = new PointF();

        private readonly Paint paint = new Paint(PaintFlags.AntiAlias)
        {
            TextAlign = Paint.Align.Left
        };

        public ClockDial(PointF dialCenter, float textSize, Color textColor, float textRadius)
        {
            this.dialCenter = dialCenter;
            this.textRadius = textRadius;
            paint.TextSize = textSize;
            paint.Color = textColor;
            paint.SetTypeface(Typeface.Create("sans-serif", TypefaceStyle.Normal));
        }

        public void OnDraw(Canvas canvas)
        {
            for (var minute = 0f; minute < MinutesInAnHour; minute += 5)
            {
                var angle = FullCircle * (minute / MinutesInAnHour) - angleOffsetCorrection;
                drawMinuteNumber(canvas, minute, (float)angle);
            }
        }

        private void drawMinuteNumber(Canvas canvas, float number, float angle)
        {
            var minuteText = number.ToString().PadLeft(digitsCount, numberPaddingChar);
            textCenter.UpdateWith(PointOnCircumference(dialCenter.ToPoint(), angle, textRadius));
            paint.GetTextBounds(minuteText, 0, minuteText.Length, textBounds);
            var centeredTextX = textCenter.X - textBounds.Width() / 2f;
            var centeredTextY = textCenter.Y + textBounds.Height() / 2f;
            canvas.DrawText(minuteText, centeredTextX, centeredTextY, paint);
        }
    }
}
