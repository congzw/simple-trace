using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public class DelayedGroupCache<T>
    {
        protected readonly object Lock = new object();

        public DelayedGroupCache()
        {
            DelayedGroups = new ConcurrentDictionary<string, DelayedGroup<T>>(StringComparer.OrdinalIgnoreCase);
            DelaySpan = TimeSpan.FromSeconds(10);
        }

        public TimeSpan DelaySpan { get; set; }

        public IDictionary<string, DelayedGroup<T>> DelayedGroups { get; set; }
        
        public void AppendToGroups(IList<T> items, Func<T, string> getGroupKey, Func<T, DateTime> getAppendAt)
        {
            //在 System.Collections.Concurrent.ConcurrentDictionary`2.TryGetValue(TKey key, TValue & value)
            //在 Common.DelayedGroupCache`1.AppendToGroups(IList`1 items, Func`2 getGroupKey, Func`2 getAppendAt)
            //在 SimpleTrace.TraceClients.ScheduleTasks.DelayedGroupCacheCommand.AppendToGroups(IList`1 items)
            //在 SimpleTrace.TraceClients.ScheduleTasks.CommandQueueTask.< Process > d__2.MoveNext()
            //        -- - 引发异常的上一位置中堆栈跟踪的末尾-- -
            //    在 System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
            //在 System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
            //在 System.Runtime.CompilerServices.TaskAwaiter.GetResult()
            //在 Demo.WinApp.UI.TraceClientsFormCtrl.< SaveQueue > d__1.MoveNext() 位置 D:\WS_Github\congzw\simple - trace\src\Demo.WinApp\UI\TraceClientsFormCtrl.cs:行号 43
            //        -- - 引发异常的上一位置中堆栈跟踪的末尾-- -
            //    在 System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
            //在 System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
            //在 System.Runtime.CompilerServices.TaskAwaiter.GetResult()
            //在 Demo.WinApp.TraceClientsForm.< btnSave_Click > d__15.MoveNext() 位置 D:\WS_Github\congzw\simple - trace\src\Demo.WinApp\TraceClientsForm.cs:行号 96

            if (getGroupKey == null)
            {
                throw new ArgumentNullException(nameof(getGroupKey));
            }

            if (getAppendAt == null)
            {
                throw new ArgumentNullException(nameof(getAppendAt));
            }

            if (items == null || items.Count == 0)
            {
                return;
            }

            lock (Lock)
            {
                var groups = items.GroupBy(getGroupKey);
                foreach (var itemGroup in groups)
                {
                    var groupKey = itemGroup.Key;
                    var groupItems = itemGroup.ToList();
                    if (groupItems.Count > 0)
                    {
                        var lastItemDate = groupItems.Max(getAppendAt);
                        DelayedGroups.TryGetValue(groupKey, out var theGroup);
                        if (theGroup == null)
                        {
                            theGroup = DelayedGroup<T>.Create(groupKey, lastItemDate);
                            DelayedGroups.Add(groupKey, theGroup);
                        }
                        theGroup.LastItemDate = lastItemDate;
                        theGroup.AppendItems(groupItems);
                    }
                }
            }
        }
        
        public IList<DelayedGroup<T>> PopExpiredGroups(DateTime popAt)
        {
            lock (Lock)
            {
                var expiredGroups = DelayedGroups.Where(x => x.Value.LastItemDate.Add(DelaySpan) <= popAt).ToList();
                foreach (var expiredGroup in expiredGroups)
                {
                    DelayedGroups.Remove(expiredGroup);
                }
                return expiredGroups.Select(x => x.Value).ToList();
            }
        }
    }

    public class DelayedGroup<T>
    {
        public DelayedGroup()
        {
            Items = new List<T>();
        }

        public string GroupKey { get; set; }
        public DateTime LastItemDate { get; set; }
        public IList<T> Items { get; set; }

        #region for easy use

        public DelayedGroup<T> AppendItem(T item)
        {
            if (!Items.Contains(item))
            {
                Items.Add(item);
            }
            return this;
        }
        public DelayedGroup<T> AppendItems(IEnumerable<T> items)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    AppendItem(item);
                }
            }
            return this;
        }
        public static DelayedGroup<T> Create(string groupKey, DateTime createAt)
        {
            var theGroup = new DelayedGroup<T>();
            theGroup.GroupKey = groupKey;
            theGroup.LastItemDate = createAt;
            return theGroup;
        }
        

        #endregion
    }
}
