using System;
using Foundation;

namespace Toggl.iOS.Shared.Extensions
{
    public static class NSDictionaryExtensions
    {
        public static void SetBoolForKey(this NSMutableDictionary dict, bool value, string key)
        {
            dict.SetValueForKey(new NSNumber(value), new NSString(key));
        }

        public static bool? GetBoolForKey(this NSDictionary dict, string key)
        {
            var value = dict.ValueForKey(new NSString(key));
            return value == null ? null as bool? : (value as NSNumber).BoolValue;
        }

        public static bool GetBoolForKey(this NSDictionary dict, string key, bool defaultValue)
        {
            var value = dict.ValueForKey(new NSString(key));
            return value == null ? defaultValue : (value as NSNumber).BoolValue;
        }

        public static void SetLongForKey(this NSMutableDictionary dict, long value, string key)
        {
            dict.SetValueForKey(new NSNumber(value), new NSString(key));
        }

        public static long? GetLongForKey(this NSDictionary dict, string key)
        {
            var value = dict.ValueForKey(new NSString(key));
            return value == null ? null as long? : (value as NSNumber).LongValue;
        }

        public static long GetLongForKey(this NSDictionary dict, string key, long defaultValue)
        {
            var value = dict.ValueForKey(new NSString(key));
            return value == null ? defaultValue : (value as NSNumber).LongValue;
        }

        public static void SetDateTimeOffsetForKey(this NSMutableDictionary dict, DateTimeOffset value, string key)
        {
            dict.SetValueForKey(new NSNumber(value.ToUnixTimeSeconds()), new NSString(key));
        }

        public static DateTimeOffset? GetDateTimeOffsetForKey(this NSDictionary dict, string key)
        {
            var value = dict.ValueForKey(new NSString(key));
            if (value == null)
                return null;

            var unixTimeStamp = (value as NSNumber).Int64Value;
            return DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp);
        }

        public static DateTimeOffset GetDateTimeOffsetForKey(this NSDictionary dict, string key, DateTimeOffset defaultValue)
        {
            var value = dict.ValueForKey(new NSString(key));
            if (value == null)
                return defaultValue;

            var unixTimeStamp = (value as NSNumber).Int64Value;
            return DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp);
        }

        public static void SetStringForKey(this NSMutableDictionary dict, string value, string key)
        {
            dict.SetValueForKey(new NSString(value), new NSString(key));
        }

        public static string GetStringForKey(this NSDictionary dict, string key, string defaultValue = null)
        {
            var value = dict.ValueForKey(new NSString(key));
            return value == null ? defaultValue : (value as NSString).ToString();
        }
    }
}
