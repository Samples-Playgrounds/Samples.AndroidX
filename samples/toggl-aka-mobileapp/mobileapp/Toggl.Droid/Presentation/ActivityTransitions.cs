namespace Toggl.Droid.Presentation
{
    public struct ActivityTransitionSet
    {
        public int SelfIn { get; }

        public int SelfOut { get; }

        public int OtherIn { get; }

        public int OtherOut { get; }

        public ActivityTransitionSet(int selfIn, int otherOut, int otherIn, int selfOut)
        {
            SelfIn = selfIn;
            SelfOut = selfOut;
            OtherIn = otherIn;
            OtherOut = otherOut;
        }
    }

    public static class Transitions
    {
        public static ActivityTransitionSet SlideInFromRight
            => new ActivityTransitionSet(
                Resource.Animation.abc_slide_in_right, Resource.Animation.abc_fade_out,
                Resource.Animation.abc_fade_in, Resource.Animation.abc_slide_out_right
            );

        public static ActivityTransitionSet SlideInFromBottom
            => new ActivityTransitionSet(
                Resource.Animation.abc_slide_in_bottom, Resource.Animation.abc_fade_out,
                Resource.Animation.abc_fade_in, Resource.Animation.abc_slide_out_bottom
            );

        public static ActivityTransitionSet Fade
            => new ActivityTransitionSet(
                Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out,
                Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out
            );
    }
}
