using Foundation;
using System;
using System.Linq;
using Toggl.Core.Services;

namespace Toggl.iOS.Services
{
    internal sealed class UserDefaultsStorageIos : KeyValueStorage
    {
        public override bool GetBool(string key)
            => NSUserDefaults.StandardUserDefaults.BoolForKey(key);

        public override string GetString(string key)
            => NSUserDefaults.StandardUserDefaults.StringForKey(key);

        public override void SetBool(string key, bool value)
        {
            NSUserDefaults.StandardUserDefaults.SetBool(value, key);
        }

        public override void SetString(string key, string value)
        {
            NSUserDefaults.StandardUserDefaults.SetString(value, key);
        }

        public override void SetInt(string key, int value)
            => NSUserDefaults.StandardUserDefaults.SetInt(value, key);

        public override void SetLong(string key, long value)
            => NSUserDefaults.StandardUserDefaults.SetValueForKey(new NSNumber(value), new NSString(key));

        public override int GetInt(string key, int defaultValue)
        {
            var objectForKey = NSUserDefaults.StandardUserDefaults.ValueForKey(new NSString(key));
            if (objectForKey == null)
                return defaultValue;

            return (int)NSUserDefaults.StandardUserDefaults.IntForKey(key);
        }

        public override long GetLong(string key, long defaultValue)
        {
            return getNumber(key)?.LongValue ?? defaultValue;
        }

        public override void Remove(string key)
        {
            NSUserDefaults.StandardUserDefaults.RemoveObject(key);
        }

        public override void RemoveAllWithPrefix(string prefix)
        {
            var keys = NSUserDefaults.StandardUserDefaults
                .ToDictionary()
                .Keys
                .Select(key => key.ToString())
                .Where(key => key.StartsWith(prefix, StringComparison.Ordinal));

            foreach (var key in keys)
            {
                Remove(key);
            }
        }

        private NSNumber getNumber(string key)
            => NSUserDefaults.StandardUserDefaults.ValueForKey(new NSString(key)) as NSNumber;
    }
}
