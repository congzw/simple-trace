using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
// ReSharper disable CheckNamespace

namespace SimpleTrace.Common
{
    public class DictionaryHelper
    {
        public static IDictionary<string, T> CreateDictionary<T>(bool ignoreCase = true, bool concurrent = false)
        {
            if (concurrent)
            {
                return ignoreCase ? new ConcurrentDictionary<string, T>(StringComparer.OrdinalIgnoreCase) : new ConcurrentDictionary<string, T>();
            }

            return ignoreCase ? new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase) : new Dictionary<string, T>();
        }
    }
}