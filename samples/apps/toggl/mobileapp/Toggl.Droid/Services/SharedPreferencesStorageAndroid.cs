using Android.Content;
using System;
using System.Linq;
using Toggl.Core.Services;

namespace Toggl.Droid.Services
{
    public sealed class SharedPreferencesStorageAndroid : KeyValueStorage
    {
        private readonly ISharedPreferences sharedPreferences;

        public SharedPreferencesStorageAndroid(ISharedPreferences sharedPreferences)
        {
            this.sharedPreferences = sharedPreferences;
        }

        public override bool GetBool(string key)
            => sharedPreferences.GetBoolean(key, false);

        public override string GetString(string key)
            => sharedPreferences.GetString(key, null);

        public override void SetBool(string key, bool value)
        {
            var editor = sharedPreferences.Edit();
            editor.PutBoolean(key, value);
            editor.Commit();
        }

        public override void SetString(string key, string value)
        {
            var editor = sharedPreferences.Edit();
            editor.PutString(key, value);
            editor.Commit();
        }

        public override void SetInt(string key, int value)
        {
            var editor = sharedPreferences.Edit();
            editor.PutInt(key, value);
            editor.Commit();
        }

        public override void SetLong(string key, long value)
        {
            var editor = sharedPreferences.Edit();
            editor.PutLong(key, value);
            editor.Commit();
        }

        public override int GetInt(string key, int defaultValue)
            => sharedPreferences.GetInt(key, defaultValue);

        public override long GetLong(string key, long defaultValue)
            => sharedPreferences.GetLong(key, defaultValue);

        public override void Remove(string key)
        {
            var editor = sharedPreferences.Edit();
            editor.Remove(key);
            editor.Commit();
        }

        public override void RemoveAllWithPrefix(string prefix)
        {
            var keys = sharedPreferences.All.Keys
                .Where(key => key.StartsWith(prefix, StringComparison.Ordinal));

            var editor = sharedPreferences.Edit();

            foreach (var key in keys)
            {
                editor.Remove(key);
            }

            editor.Commit();
        }
    }
}
