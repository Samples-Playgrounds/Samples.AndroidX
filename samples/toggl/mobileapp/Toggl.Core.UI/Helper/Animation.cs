namespace Toggl.Core.UI.Helper
{
    public static class Animation
    {
        public struct CubicBezierCurve
        {
            public CubicBezierCurve(float p0, float p1, float p2, float p3)
            {
                P0 = p0;
                P1 = p1;
                P2 = p2;
                P3 = p3;
            }

            public float P0 { get; }
            public float P1 { get; }
            public float P2 { get; }
            public float P3 { get; }
        }

        public static class Curves
        {
            public static readonly CubicBezierCurve StandardEase = new CubicBezierCurve(0.4f, 0.0f, 0.2f, 1.0f);
            public static readonly CubicBezierCurve EaseOut = new CubicBezierCurve(0.0f, 0.0f, 0.2f, 1.0f);
            public static readonly CubicBezierCurve EaseIn = new CubicBezierCurve(0.4f, 0.0f, 1.0f, 1.0f);
            public static readonly CubicBezierCurve SharpCurve = new CubicBezierCurve(0.4f, 0.0f, 0.6f, 1.0f);
            public static readonly CubicBezierCurve Bounce = new CubicBezierCurve(0.175f, 0.885f, 0.32f, 1.375f);
            public static readonly CubicBezierCurve CardInCurve = new CubicBezierCurve(0.23f, 1.0f, 0.32f, 1.0f);
            public static readonly CubicBezierCurve CardOutCurve = new CubicBezierCurve(0.86f, 0.0f, 0.07f, 1.0f);
            public static readonly CubicBezierCurve Linear = new CubicBezierCurve(0.0f, 0.0f, 1.0f, 1.0f);
        }

        public static class Timings
        {
            public const float SpiderBro = 1.5f;

            public const float ReportsLoading = 4.0f;

            public const float EnterTiming = 0.225f;
            public const float LeaveTiming = 0.195f;
            public const float LeaveTimingFaster = 0.145f;

            public const int HideSyncStateViewDelay = 2000;
        }
    }
}
