using Android.Views;

namespace Toggl.Droid.Extensions
{
    public static class VisibilityExtensions
    {
        public static bool ToBool(this ViewStates state)
            => state == ViewStates.Visible;

        public static ViewStates ToVisibility(this bool state, bool useGone = true)
            => state ? ViewStates.Visible : (useGone ? ViewStates.Gone : ViewStates.Invisible);
    }
}
