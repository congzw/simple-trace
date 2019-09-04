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
        private readonly KnownCommands _knownCommands;

        public CommandQueueTask(DelayedGroupCacheCommand delayedGroupCacheCommand, KnownCommands knownCommands)
        {
            _delayedGroupCacheCommand = delayedGroupCacheCommand;
            _knownCommands = knownCommands;
        }

        public IList<IClientSpan> GetEntities(IList<Command> allCommands, DateTime now)
        {
            var spanCache = new Dictionary<string, IClientSpan>();
            var commandLogistics = _knownCommands.CommandLogistics;
            var logistics = commandLogistics.OrderBy(x => x.ProcessSort).ToList();

            //process no delay
            var noDelayLogistics = commandLogistics.Where(x => !x.NeedDelay).ToList();
            foreach (var noDelayLogistic in noDelayLogistics)
            {
                var noDelayCommands = allCommands.Where(x => noDelayLogistic.IsForCommand(x)).ToList();
                foreach (var noDelayCommand in noDelayCommands)
                {
                    noDelayLogistic.CreateOrUpdate(noDelayCommand, spanCache);
                }
            }

            //process delay
            var delayLogistics = logistics.Where(x => x.NeedDelay).ToList();
            foreach (var delayLogistic in delayLogistics)
            {
                var delayCommands = allCommands.Where(x => delayLogistic.IsForCommand(x)).ToList();
                _delayedGroupCacheCommand.AppendToGroups(delayCommands);
            }

            var expiredGroups = _delayedGroupCacheCommand.PopExpiredGroups(now).OrderBy(x => x.LastItemDate).ToList();
            if (expiredGroups.Count > 0)
            {
                foreach (var expiredGroup in expiredGroups)
                {
                    foreach (var delayCommand in expiredGroup.Items)
                    {
                        foreach (var delayLogistic in delayLogistics)
                        {
                            delayLogistic.CreateOrUpdate(delayCommand, spanCache);
                        }
                    }
                }
            }

            var clientSpanEntities = spanCache.Values.ToList();
            return clientSpanEntities;
        }

        public async Task Process(IList<IClientSpanProcess> processes, CommandQueue commandQueue, DateTime now)
        {
            //process steps:
            //dequeue all commands to groupCache
            //get expired entities from commands
            //run process: save client spans
            //run process: send client spans
            //run process: ...

            var currentCommands = await commandQueue.TryDequeueAll().ConfigureAwait(false);
            var spanEntities = GetEntities(currentCommands, now);

            var orderedProcesses = processes.OrderBy(x => x.SortNum).ToList();
            await Task.WhenAll(orderedProcesses.Select(x => x.Process(spanEntities)));
        }
    }
}
