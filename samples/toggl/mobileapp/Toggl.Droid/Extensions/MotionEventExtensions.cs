using Android.Graphics;
using Android.Views;

namespace Toggl.Droid.Extensions
{
    public static class MotionEventExtensions
    {
        public static PointF ToPointF(this MotionEvent motionEvent)
            => new PointF(motionEvent.GetX(), motionEvent.GetY());

        public static void UpdateWith(this PointF updatingPoint, MotionEvent motionEvent)
        {
            updatingPoint.Set(motionEvent.GetX(), motionEvent.GetY());
        }
    }
}
