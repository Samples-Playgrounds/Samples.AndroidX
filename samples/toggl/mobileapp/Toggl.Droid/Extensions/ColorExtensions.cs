using Toggl.Shared;
using AndroidColor = Android.Graphics.Color;

namespace Toggl.Droid.Extensions
{
    public static class ColorExtensions
    {
        public static AndroidColor ToNativeColor(this Color color)
            => new AndroidColor(color.Red, color.Green, color.Blue, color.Alpha);
    }
}
