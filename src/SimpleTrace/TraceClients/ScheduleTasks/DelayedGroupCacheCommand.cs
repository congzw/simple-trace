using System;
using System.Collections.Generic;
using Common;
using SimpleTrace.TraceClients.Commands;

namespace SimpleTrace.TraceClients.ScheduleTasks
{
    public class DelayedGroupCacheCommand : DelayedGroupCache<Command>
    {
        public DelayedGroupCacheCommand()
        {
            DelaySpan = TimeSpan.FromSeconds(5);
        }

        public void AppendToGroups(IList<Command> items)
        {
            AppendToGroups(items, item => item.TryGetTraceId(), item => item.CreateUtc);
        }
    }
}