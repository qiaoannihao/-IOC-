using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleIoc
{
    public static class DictionaryExtension
    {
        #region Dictionary
        public static IDictionary<TKey, TValue> AddAndSet<TKey, TValue>(this IDictionary<TKey, TValue> keyValuePairs, TKey key, TValue value)
        {
            if (keyValuePairs.ContainsKey(key))
            {
                keyValuePairs[key] = value;
            }
            else
            {
                keyValuePairs.Add(key, value);
            }
            return keyValuePairs;
        }

        public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> keyValuePairs, TKey key)
        {
            if (keyValuePairs.ContainsKey(key))
            {
                return keyValuePairs[key];
            }
            else
            {
                return default(TValue);
            }
        }

        public static IDictionary<TKey, TValue> RemoveObject<TKey, TValue>(this IDictionary<TKey, TValue> keyValuePairs, TKey key, TValue value)
        {
            if (keyValuePairs.ContainsKey(key))
            {
                keyValuePairs.Remove(key);
            }
            return keyValuePairs;
        }
        #endregion
    }
}
