using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Toggl.Shared.Extensions.RxAction
{
    public static class NameValueCollectionExtensions
    {
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this NameValueCollection nvc, Func<string, TKey> extractKey, Func<string, TValue> extractValue)
        {
            var result = new Dictionary<TKey, TValue>();

            foreach (var keyString in nvc.AllKeys)
            {
                var key = extractKey(keyString);
                var value = extractValue(nvc[keyString]);

                result.Add(key, value);
            }

            return result;
        }
    }
}
