//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using SimpleTrace.TraceClients.Commands;

//namespace SimpleTrace.TraceClients.ScheduleTasks
//{
//    public class CommandQueueTask
//    {
//        private readonly DelayedGroupCacheCommand _delayedGroupCacheCommand;

//        public CommandQueueTask(DelayedGroupCacheCommand delayedGroupCacheCommand)
//        {
//            _delayedGroupCacheCommand = delayedGroupCacheCommand;
//        }

//        public IList<ClientSpanEntity> GetEntities(IList<ICommand> allCommands, DateTime now)
//        {
//            var spanCache = new Dictionary<string, ClientSpanEntity>();
//            var theType = typeof(SaveSpansCommand);
//            var noDelayCommands = allCommands.Where(x => x.GetType() == theType).ToList();
//            foreach (var noDelayCommand in noDelayCommands)
//            {
//                noDelayCommand.CreateOrUpdate(spanCache);
//            }

//            var delayedCommands = allCommands.Where(x => x.GetType() != theType).ToList();
//            _delayedGroupCacheCommand.AppendToGroups(delayedCommands);
//            var expiredGroups = _delayedGroupCacheCommand.PopExpiredGroups(now).OrderBy(x => x.LastItemDate).ToList();
//            if (expiredGroups.Count > 0)
//            {
//                var expiredCommands = new List<ICommand>();
//                foreach (var expiredGroup in expiredGroups)
//                {
//                    foreach (var cmd in expiredGroup.Items)
//                    {
//                        expiredCommands.Add(cmd);
//                    }
//                }

//                var orderedCommandGroups = expiredCommands.GroupBy(x => x.ProcessSort).OrderBy(g => g.Key);
//                foreach (var commandGroup in orderedCommandGroups)
//                {
//                    var theCommands = commandGroup.ToList();
//                    foreach (var theCommand in theCommands)
//                    {
//                        theCommand.CreateOrUpdate(spanCache);
//                    }
//                }
//            }

//            var clientSpanEntities = spanCache.Values.ToList();
//            return clientSpanEntities;
//        }

//        public async Task Process(IList<IClientSpanProcess> processes, CommandQueue commandQueue, DateTime now)
//        {
//            //process steps:
//            //dequeue all commands to groupCache
//            //get expired entities from commands
//            //run process: save client spans
//            //run process: send client spans
//            //run process: ...

//            var currentCommands = await commandQueue.TryDequeueAll().ConfigureAwait(false);
//            var spanEntities = GetEntities(currentCommands, now);

//            var orderedProcesses = processes.OrderBy(x => x.SortNum).ToList();
//            await Task.WhenAll(orderedProcesses.Select(x => x.Process(spanEntities)));
//        }
//    }
//}
