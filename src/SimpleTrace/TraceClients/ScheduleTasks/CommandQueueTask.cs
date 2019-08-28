using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimpleTrace.TraceClients.Commands;

namespace SimpleTrace.TraceClients.ScheduleTasks
{
    public class CommandQueueTask
    {
        private readonly DelayedGroupCacheCommand _delayedGroupCacheCommand;

        public CommandQueueTask(DelayedGroupCacheCommand delayedGroupCacheCommand)
        {
            _delayedGroupCacheCommand = delayedGroupCacheCommand;
        }

        public async Task Process(IList<IClientSpanProcess> processes, CommandQueue commandQueue, DateTime now)
        {
            //process steps:
            //dequeue all commands to groupCache
            //get expired commands from groupCache
            //run process: save client spans
            //run process: send client spans
            //run process: ...

            var currentCommands = await commandQueue.TryDequeueAll().ConfigureAwait(false);
            _delayedGroupCacheCommand.AppendToGroups(currentCommands);

            var expiredGroups = _delayedGroupCacheCommand.PopExpiredGroups(now).OrderBy(x => x.LastItemDate).ToList();
            if (expiredGroups.Count == 0)
            {
                return;
            }

            var commands = new List<ICommand>();
            foreach (var expiredGroup in expiredGroups)
            {
                foreach (var cmd in expiredGroup.Items)
                {
                    commands.Add(cmd);
                }
            }

            var spanCache = new Dictionary<string, ClientSpanEntity>();

            var commandGroups = commands.GroupBy(x => x.ProcessSort).OrderBy(g => g.Key);
            foreach (var commandGroup in commandGroups)
            {
                var theCommands = commandGroup.ToList();
                foreach (var theCommand in theCommands)
                {
                    theCommand.CreateOrUpdate(spanCache);
                }
            }

            var clientSpanEntities = spanCache.Values.ToList();

            var orderedProcesses = processes.OrderBy(x => x.SortNum).ToList();
            
            await Task.WhenAll(orderedProcesses.Select(x => x.Process(clientSpanEntities)));
        }
    }
}
