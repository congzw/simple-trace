// ReSharper disable CheckNamespace

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Common
{
    public class ObjectCounter: IDisposable
    {
        private readonly object _lock = new object();

        public ObjectCounter(bool enabled)
        {
            Enabled = enabled;
            Items = new ConcurrentDictionary<Type, int>();
            RecordAdd(this);
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
                System.Diagnostics.Trace.WriteLine(string.Format("[RecordDelete]:{0} <<{1}:{2}>>", count, theType.Name, instance.GetHashCode()));
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
                System.Diagnostics.Trace.WriteLine(string.Format("[RecordAdd]:{0} <<{1}:{2}>>", count, theType.Name, instance.GetHashCode()));
            }
        }

        public bool Enabled { get; set; }

        public static ObjectCounter Instance = new ObjectCounter(true);

        public void Dispose()
        {
            RecordDelete(this);
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
