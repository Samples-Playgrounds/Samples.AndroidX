using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using System;

namespace Toggl.Droid.Views
{
    [Register("toggl.droid.views.ValueSlider")]
    public sealed class ValueSlider : SeekBar
    {
        private float hue;
        public float Hue
        {
            get => hue;
            set
            {
                if (hue == value) return;
                hue = value;
                updateValues();
            }
        }

        private float saturation;
        public float Saturation
        {
            get => saturation;
            set
            {
                if (saturation == value) return;
                saturation = value;
                updateValues();
            }
        }

        public ValueSlider(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public ValueSlider(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            updateValues();
        }

        private void updateValues()
        {
            var colors = new[]
            {
                Color.HSVToColor(new [] { hue * 360, saturation, 1.0f }).ToArgb(),
                Color.HSVToColor(new [] { hue * 360, saturation, 0.25f }).ToArgb()
            };

            ProgressDrawable = new GradientDrawable(GradientDrawable.Orientation.LeftRight, colors);
        }
    }
}
