using Android.Graphics;
using Android.Graphics.Drawables;

namespace Toggl.Droid.Extensions
{
    public static class DrawableExtensions
    {
        public static Bitmap ToBitmap(this VectorDrawable vectorDrawable, int? width = null, int? height = null)
        {
            var bitmap = Bitmap.CreateBitmap(
                width ?? vectorDrawable.IntrinsicWidth,
                height ?? vectorDrawable.IntrinsicHeight,
                Bitmap.Config.Argb8888);
            var canvas = new Canvas(bitmap);
            vectorDrawable.SetBounds(0, 0, canvas.Width, canvas.Height);
            vectorDrawable.Draw(canvas);
            return bitmap;
        }
    }
}
