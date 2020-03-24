using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using System;
using AndroidX.Core.Graphics;
using Toggl.Droid.Extensions;
using Toggl.Shared.Extensions;
using static System.Math;

namespace Toggl.Droid.Views
{
    [Register("toggl.droid.views.HueSaturationPickerView")]
    public sealed class HueSaturationPickerView : View
    {
        private static readonly int[] brightnessGradientColors = { Color.Transparent.ToArgb(), Color.White.ToArgb() };
        private static readonly int[] colorGradientColors =
        {
            Color.Red.ToArgb(), Color.Yellow.ToArgb(), Color.Green.ToArgb(),
            Color.Cyan.ToArgb(), Color.Blue.ToArgb(), Color.Magenta.ToArgb(), Color.Red.ToArgb()
        };

        private readonly int circleRadius;
        private readonly int circleDiameter;
        private readonly Paint circleFillPaint;
        private readonly Paint circleStrokePaint;
        private static readonly Color circleColor = Color.White;

        private readonly Paint brightnessPaint;
        private readonly Paint opacityBackgroundPaint;
        private readonly Paint gradientBackgroundPaint;

        public event EventHandler HueChanged;
        public event EventHandler SaturationChanged;

        public float Hue { get; set; }

        public float Saturation { get; set; }

        private float value;
        public float Value
        {
            get => value;
            set
            {
                if (this.value == value) return;
                this.value = value;

                var opacity = complement(value) * 255;
                opacityBackgroundPaint.Color = new Color(ColorUtils.SetAlphaComponent(Color.Black.ToArgb(), (int)opacity));
                Invalidate();
            }
        }

        public HueSaturationPickerView(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public HueSaturationPickerView(Context context, IAttributeSet attrs)
            : this(context, attrs, 0)
        {
        }

        public HueSaturationPickerView(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            circleDiameter = 30.DpToPixels(context);
            circleRadius = circleDiameter / 2;

            circleFillPaint = new Paint();
            circleFillPaint.SetStyle(Paint.Style.Fill);
            circleFillPaint.Flags = PaintFlags.AntiAlias;

            circleStrokePaint = new Paint();
            circleStrokePaint.SetStyle(Paint.Style.Stroke);
            circleStrokePaint.Color = circleColor;
            circleStrokePaint.StrokeCap = Paint.Cap.Round;
            circleStrokePaint.Flags = PaintFlags.AntiAlias;
            circleStrokePaint.StrokeWidth = 2.DpToPixels(Context);

            gradientBackgroundPaint = new Paint();
            gradientBackgroundPaint.SetStyle(Paint.Style.Fill);
            gradientBackgroundPaint.Flags = PaintFlags.AntiAlias;

            opacityBackgroundPaint = new Paint();
            opacityBackgroundPaint.SetStyle(Paint.Style.Fill);
            opacityBackgroundPaint.Flags = PaintFlags.AntiAlias;

            brightnessPaint = new Paint();
            brightnessPaint.SetStyle(Paint.Style.Fill);
            brightnessPaint.Flags = PaintFlags.AntiAlias;
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                case MotionEventActions.Move:
                    updateLocation(e.GetX(), e.GetY());
                    return true;
            }

            return base.OnTouchEvent(e);
        }

        private void updateLocation(float x, float y)
        {
            var width = Width;
            var height = Height;
            var pointX = x.Clamp(0, width);
            var pointY = y.Clamp(0, height);

            Hue = pointX / width;
            Saturation = complement(pointY / height);

            HueChanged?.Invoke(this, new EventArgs());
            SaturationChanged?.Invoke(this, new EventArgs());

            Invalidate();
        }

        public override void Draw(Canvas canvas)
        {
            base.Draw(canvas);

            var colorGradient = new LinearGradient(0, 0, Width, 0, colorGradientColors, null, Shader.TileMode.Clamp);
            gradientBackgroundPaint.SetShader(colorGradient);

            var brightnessGradient = new LinearGradient(0, 0, 0, Height, brightnessGradientColors, null, Shader.TileMode.Clamp);
            brightnessPaint.SetShader(brightnessGradient);

            circleFillPaint.Color = Color.HSVToColor(new[] { Hue * 360, Saturation, Value });

            canvas.DrawPaint(gradientBackgroundPaint);
            canvas.DrawPaint(brightnessPaint);
            canvas.DrawPaint(opacityBackgroundPaint);

            var x = (Width * Hue) - circleRadius;
            var y = Height * complement(Saturation) - circleRadius;

            var circle = new RectF(x, y, x + circleDiameter, y + circleDiameter);
            canvas.DrawRoundRect(circle, circleRadius, circleRadius, circleStrokePaint);
            canvas.DrawRoundRect(circle, circleRadius, circleRadius, circleFillPaint);
        }

        private float complement(float number) => Abs(number - 1);
    }
}
