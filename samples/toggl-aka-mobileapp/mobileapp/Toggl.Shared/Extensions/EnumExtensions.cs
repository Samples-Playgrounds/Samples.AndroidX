using System;

namespace Toggl.Shared.Extensions
{
    public static class EnumExtensions
    {
        public static T NextEnumValue<T>(this T enumValue) where T : Enum
        {
            if (!typeof(T).IsEnum) throw new ArgumentException($"Argument {typeof(T).FullName} is not an Enum");

            var enumValues = (T[])Enum.GetValues(typeof(T));
            var nextIndex = Array.IndexOf(enumValues, enumValue) + 1;
            return enumValues[nextIndex % enumValues.Length];
        }
    }
}
