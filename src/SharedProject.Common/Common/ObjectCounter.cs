// ReSharper disable CheckNamespace

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Common
{
    public sealed class ObjectCounter: IDisposable
    {
        private readonly object _lock = new object();

        public ObjectCounter(bool enabled)
        {
            Enabled = enabled;
            Items = new ConcurrentDictionary<Type, int>();
        }

        public IDictionary<Type, int> Items { get; set; }

        public void RecordDelete(object instance)
        {
            if (!Enabled)
            {
                return;
            }

            if (instance == null)
            {
                return;
            }

            lock (_lock)
            {
                var theType = instance.GetType();
                int count;
                if (!Items.ContainsKey(theType))
                {
                    count = 0;
                }
                else
                {
                    count = Items[theType] - 1;
                }
                Items[theType] = count;
                LogInfo(string.Format("[RecordDelete]:{0} <<{1}:{2}>>", count, theType.Name, instance.GetHashCode()));
            }
        }

        public void RecordAdd(object instance)
        {
            if (!Enabled)
            {
                return;
            }

            if (instance == null)
            {
                return;
            }

            lock (_lock)
            {
                var theType = instance.GetType();
                int count;
                if (!Items.ContainsKey(theType))
                {
                    count = 1;
                }
                else
                {
                    count = Items[theType] + 1;
                }
                Items[theType] = count;
                LogInfo(string.Format("[RecordAdd]:{0} <<{1}:{2}>>", count, theType.Name, instance.GetHashCode()));
            }
        }

        public bool Enabled { get; set; }

        public static ObjectCounter Instance = new ObjectCounter(true);

        public void Dispose()
        {
        }

        private void LogInfo(string info)
        {
            //dead loop!
            //var logger = SimpleLogSingleton<Object>.Instance.Logger;
            //logger.LogInfo(info);
            System.Diagnostics.Trace.WriteLine(info);
        }

    }

    public static class ObjectCounterExtensions
    {
        public static void ReportAdd(this object instance)
        {
            ObjectCounter.Instance.RecordAdd(instance);
        }

        public static void ReportDelete(this object instance)
        {
            ObjectCounter.Instance.RecordDelete(instance);
        }
    }
}
