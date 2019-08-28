using System;
using System.Collections.Generic;
using SimpleTrace.Common;
using SimpleTrace.TraceClients.Commands;

namespace SimpleTrace.TraceClients.ScheduleTasks
{
    public class DelayedGroupCacheCommand : DelayedGroupCache<ICommand>
    {
        public DelayedGroupCacheCommand()
        {
            DelaySpan = TimeSpan.FromSeconds(5);
        }

        public void AppendToGroups(IList<ICommand> items)
        {
            AppendToGroups(items, item => item.TryGetTraceId(), item => item.CreateUtc);
        }
    }
}