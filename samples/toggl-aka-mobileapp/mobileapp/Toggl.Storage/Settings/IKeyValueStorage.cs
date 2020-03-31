using System;

namespace Toggl.Storage.Settings
{
    public interface IKeyValueStorage
    {
        bool GetBool(string key);

        string GetString(string key);

        int GetInt(string key, int defaultValue);

        long GetLong(string key, long defaultValue);

        DateTimeOffset? GetDateTimeOffset(string key);

        TimeSpan? GetTimeSpan(string key);

        void SetBool(string key, bool value);

        void SetString(string key, string value);

        void SetInt(string key, int value);

        void SetLong(string key, long value);

        void SetDateTimeOffset(string key, DateTimeOffset value);

        void SetTimeSpan(string key, TimeSpan timeSpan);

        void Remove(string key);

        void RemoveAllWithPrefix(string prefix);
    }
}
