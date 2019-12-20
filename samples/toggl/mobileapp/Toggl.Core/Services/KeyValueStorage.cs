using System;
using System.Globalization;
using Toggl.Storage.Settings;

namespace Toggl.Core.Services
{
    public abstract class KeyValueStorage : IKeyValueStorage
    {
        public abstract bool GetBool(string key);
        public abstract string GetString(string key);
        public abstract int GetInt(string key, int defaultValue);
        public abstract long GetLong(string key, long defaultValue);

        public abstract void SetBool(string key, bool value);
        public abstract void SetString(string key, string value);
        public abstract void SetInt(string key, int value);
        public abstract void SetLong(string key, long value);

        public abstract void Remove(string key);
        public abstract void RemoveAllWithPrefix(string prefix);

        public DateTimeOffset? GetDateTimeOffset(string key)
        {
            var serialized = GetString(key);
            if (string.IsNullOrEmpty(serialized))
                return null;

            if (long.TryParse(serialized, out var epoch))
                return DateTimeOffset.FromUnixTimeMilliseconds(epoch);

            if (DateTimeOffset.TryParse(
                serialized, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out var parsed))
            {
                // the storage contains the date in an old format, update it to use unix time
                SetDateTimeOffset(key, parsed);
                return parsed;
            }

            if (DateTimeOffset.TryParse(serialized, out var nonInvariantParsed))
            {
                // the storage contains the date in an old format, update it to use unix time
                SetDateTimeOffset(key, nonInvariantParsed);
                return nonInvariantParsed;
            }

            // the storage contains some value which is not parsable
            Remove(key);
            return null;
        }

        public void SetDateTimeOffset(string key, DateTimeOffset dateTime)
        {
            var epoch = dateTime.ToUnixTimeMilliseconds();
            SetString(key, epoch.ToString());
        }

        public TimeSpan? GetTimeSpan(string key)
        {
            var ticks = GetLong(key, -1);
            if (ticks < 0) return null;

            return TimeSpan.FromTicks(ticks);
        }

        public void SetTimeSpan(string key, TimeSpan timeSpan)
        {
            SetLong(key, timeSpan.Ticks);
        }
    }
}
