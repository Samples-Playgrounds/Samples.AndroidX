using Android.Content;

namespace Toggl.Droid.Extensions
{
    public struct PopupOffsets
    {
        public int HorizontalOffset { get; }
        public int VerticalOffset { get; }

        public PopupOffsets(int horizontalOffset, int verticalOffset)
        {
            HorizontalOffset = horizontalOffset;
            VerticalOffset = verticalOffset;
        }

        public static PopupOffsets FromDp(int width, int height, Context context)
            => new PopupOffsets(width.DpToPixels(context), height.DpToPixels(context));
    }
}
