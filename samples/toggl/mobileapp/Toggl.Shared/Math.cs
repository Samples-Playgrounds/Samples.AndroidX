using System;

namespace Toggl.Shared
{
    public static class Math
    {
        public const double QuarterOfCircle = 0.5 * System.Math.PI;

        public const double FullCircle = 2 * System.Math.PI;

        public const int HoursOnTheClock = 12;

        public const int MinutesInAnHour = 60;

        public const int SecondsInAMinute = 60;

        public static double ToAngleOnTheDial(this TimeSpan time)
            => time.ToAngle() - QuarterOfCircle;

        public static double ToAngle(this TimeSpan time)
            => (time.Minutes * SecondsInAMinute + time.Seconds) / (double)(MinutesInAnHour * SecondsInAMinute) * FullCircle;

        public static double ToPositiveAngle(this double angle)
        {
            while (angle < 0) angle += FullCircle;
            return angle;
        }

        public static TimeSpan AngleToTime(this double angle)
        {
            var time = angle / FullCircle * MinutesInAnHour;
            var minutes = (int)time;
            var seconds = (int)((time - minutes) * SecondsInAMinute);
            return new TimeSpan(0, minutes, seconds);
        }

        public static double AngleBetween(Point a, Point b)
            => System.Math.Atan2(a.Y - b.Y, a.X - b.X);

        public static double DistanceSq(Point a, Point b)
        {
            var dx = a.X - b.X;
            var dy = a.Y - b.Y;
            return dx * dx + dy * dy;
        }

        public static Point PointOnCircumference(Point center, double angle, double radius)
            => new Point
            {
                X = center.X + radius * System.Math.Cos(angle),
                Y = center.Y + radius * System.Math.Sin(angle)
            };

        public static bool IsBetween(this double angle, double startAngle, double endAngle)
        {
            angle = angle.ToPositiveAngle();
            startAngle = startAngle.ToPositiveAngle();
            endAngle = endAngle.ToPositiveAngle();
            return startAngle > endAngle
                ? startAngle <= angle || angle <= endAngle
                : startAngle <= angle && angle <= endAngle;
        }

        public static int PingPongClamp(this int number, int length)
        {
            if (length <= 0) throw new ArgumentOutOfRangeException($"The length for clamping must be at positive integer, {length} given.");
            if (number < 0) throw new ArgumentOutOfRangeException($"The clamped number a non-negative integer, {number} given.");

            if (length == 1) return 0;

            var lengthOfFoldedSequence = 2 * length - 2;
            var indexInFoldedSequence = number % lengthOfFoldedSequence;
            return indexInFoldedSequence < length ? indexInFoldedSequence : lengthOfFoldedSequence - indexInFoldedSequence;
        }
    }
}
