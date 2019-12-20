using System;
using System.Collections.Generic;
using System.Linq;

namespace Toggl.Shared.Extensions
{
    public static class DictionaryExtensions
    {
        public static long? GetValueAsLong(this Dictionary<string, string> dictionary, string key)
        {
            if (dictionary.TryGetValue(key, out var valueString) && long.TryParse(valueString, out var value))
                return value;

            return null;
        }

        public static long[] GetValueAsLongs(this Dictionary<string, string> dictionary, string key)
        {
            if (dictionary.TryGetValue(key, out var valueString))
            {
                if (valueString.StartsWith("["))
                {
                    valueString = valueString.Substring(1);
                }

                if (valueString.EndsWith("]"))
                {
                    valueString = valueString.Substring(0, valueString.Length - 1);
                }

                var values = valueString
                    .Split(',')
                    .Select(long.Parse)
                    .ToArray();

                return values;
            }

            return null;
        }

        public static DateTimeOffset? GetValueAsDateTimeOffset(this Dictionary<string, string> dictionary, string key)
        {
            if (dictionary.TryGetValue(key, out var valueString))
            {
                if (valueString.StartsWith("\""))
                {
                    valueString = valueString.Substring(1);
                }

                if (valueString.EndsWith("\""))
                {
                    valueString = valueString.Substring(0, valueString.Length - 1);
                }

                if (DateTimeOffset.TryParse(valueString, out var value))
                    return value;
            }

            return null;
        }

        public static TimeSpan? GetValueAsTimeSpan(this Dictionary<string, string> dictionary, string key)
        {
            if (dictionary.TryGetValue(key, out var valueString) && long.TryParse(valueString, out var value))
                return TimeSpan.FromSeconds(value);

            return null;
        }

        public static bool? GetValueAsBool(this Dictionary<string, string> dictionary, string key)
        {
            if (dictionary.TryGetValue(key, out var valueString) && bool.TryParse(valueString, out var value))
                return value;

            return null;
        }

        public static string GetValueAsString(this Dictionary<string, string> dictionary, string key)
        {
            if (dictionary.TryGetValue(key, out var value))
                return value;

            return null;
        }
        public static TEnum GetValueAsEnumCase<TEnum>(this Dictionary<string, string> dictionary, string key, TEnum defaultCase)
            where TEnum : struct, Enum
        {

            if (dictionary.TryGetValue(key, out var sourceString) && Enum.TryParse<TEnum>(sourceString, out var source))
                return source;

            return defaultCase;
        }

        public static U GetOrDefault<T, U>(this IDictionary<T, U> dictionary, T key, U defaultValue)
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key];
            return defaultValue;
        }
    }
}
