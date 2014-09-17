using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftServe.BL.Helpers
{
    public static class DictionaryExtentions
    {
        public static void AddOrReplase<TKey, TValue>(this IDictionary<TKey,TValue> dictionary, TKey key,TValue value)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] = value;
            else
                dictionary.Add(key, value);
        }
    }
}
