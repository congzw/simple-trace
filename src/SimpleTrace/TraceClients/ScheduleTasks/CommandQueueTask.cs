using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using SimpleTrace.TraceClients.Commands;

namespace SimpleTrace.TraceClients.ScheduleTasks
{
    public class CommandQueueTask
    {
        public CommandQueueTask()
        {
            DelayedGroupCache = new DelayedGroupCacheCommand();
        }

        public DelayedGroupCacheCommand DelayedGroupCache { get; set; }

        #region for process step by step

        public Task<IList<Command>> DequeueCommands(CommandQueue commandQueue)
        {
            return commandQueue.TryDequeueAll();
        }

        public IList<IClientSpan> GetEntities(IList<ICommandLogistic> commandLogistics, IList<Command> allCommands, DateTime now)
        {
            var spanCache = new Dictionary<string, IClientSpan>();
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
                DelayedGroupCache.AppendToGroups(delayCommands);
            }

            var expiredGroups = DelayedGroupCache.PopExpiredGroups(now).OrderBy(x => x.LastItemDate).ToList();
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

        public Task ProcessEntities(IList<IClientSpanProcess> processes, IList<IClientSpan> clientSpans)
        {
            var orderedProcesses = processes.OrderBy(x => x.SortNum).ToList();
            return Task.WhenAll(orderedProcesses.Select(x => x.Process(clientSpans)));
        }

        #endregion

        public async Task ProcessQueue(CommandQueue commandQueue, IList<ICommandLogistic> commandLogistics, IList<IClientSpanProcess> processes, DateTime now)
        {
            //process steps:
            //dequeue all commands to groupCache
            //get expired entities from commands
            //run processes:
            //  process => save client spans
            //  process => send client spans
            //  process => ...

            var currentCommands = await DequeueCommands(commandQueue).ConfigureAwait(false);
            var spanEntities = GetEntities(commandLogistics, currentCommands, now);
            LogInfo(string.Format("ProcessQueue at {0:yyyyMMddHHmmss} => Commands: {1}, Spans: {2}, Processes:{3} Thread:{4}", 
                now,
                currentCommands.Count,
                spanEntities.Count,
                processes.Count, 
                Thread.CurrentThread.ManagedThreadId), now);

            if (spanEntities.Count == 0)
            {
                return;
            }
            var orderedProcesses = processes.OrderBy(x => x.SortNum).ToList();
            await Task.WhenAll(orderedProcesses.Select(x => x.Process(spanEntities)));
        }

        private void LogInfo(string message, DateTime processAt)
        {
            CommandQueueProcessLogs.Instance.LogInfo(processAt, message);
        }
    }

    #region for debug
    
    public class CommandQueueProcessLog
    {
        public DateTime ProcessAt { get; set; }
        public string Info { get; set; }
    }

    public class CommandQueueProcessLogs
    {
        public CommandQueueProcessLogs()
        {
            Enabled = false;
            Items = new ConcurrentDictionary<DateTime, CommandQueueProcessLog>();
        }

        public bool Enabled { get; set; }

        public IDictionary<DateTime, CommandQueueProcessLog> Items { get; set; }

        public void LogInfo(DateTime now, string info)
        {
            if (!Enabled)
            {
                return;
            }

            var logger = SimpleLogSingleton<CommandQueueProcessLogs>.Instance.Logger;
            logger.LogInfo(info);
            Items[now] = new CommandQueueProcessLog() { ProcessAt = now, Info = info };
        }

        public void Clear()
        {
            Items.Clear();
        }

        public static CommandQueueProcessLogs Instance = new CommandQueueProcessLogs();
    }

    #endregion
}
