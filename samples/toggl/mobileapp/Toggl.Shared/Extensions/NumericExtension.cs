using System;

namespace Toggl.Shared.Extensions
{
    public static class NumericExtensions
    {
        public static T Clamp<T>(this T num, T min, T max) where T : IComparable
        {
            if (num.CompareTo(min) < 0) return min;
            if (num.CompareTo(max) > 0) return max;
            return num;
        }

        public static bool IsInRange(this int number, int min, int max)
            => number >= min && number <= max;
    }
}
