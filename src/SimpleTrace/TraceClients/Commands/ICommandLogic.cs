using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimpleTrace.Common;
using SimpleTrace.TraceClients.ScheduleTasks;

namespace SimpleTrace.TraceClients.Commands
{
    public class Command
    {
        public Command(object args, string commandType)
        {
            Args = args;
            ArgsType = args.GetType().Name;
            CommandType = commandType;
            CreateUtc = DateHelper.Instance.GetDateNow();
        }

        public string CommandType { get; set; }
        public string ArgsType { get; set; }
        public DateTime CreateUtc { get; set; }
        public object Args { get; set; }
    }
    
    public interface ICommandLogistic
    {
        bool NeedDelay { get; }
        float ProcessSort { get; }
        bool IsForCommand(Command command);
        bool CreateOrUpdate(Command command, IDictionary<string, ClientSpanEntity> clientSpanCache);
    }
    
    public abstract class BaseCommandLogistic<T>: ICommandLogistic where T : ICommandLogistic
    {
        protected BaseCommandLogistic(bool needDelay, float processSort)
        {
            NeedDelay = needDelay;
            ProcessSort = processSort;
            CommandType = this.GetType();
        }

        protected Type CommandType { get; set; }
        
        public float ProcessSort { get; }
        public bool IsForCommand(Command command)
        {
            return CommandType.Name == command.CommandType;
        }
        public bool NeedDelay { get; }
        public abstract bool CreateOrUpdate(Command command, IDictionary<string, ClientSpanEntity> clientSpanCache);

        public static Command Create(object args)
        {
            return new Command(args, typeof(T).Name);
        }
    }
    public class SaveSpansCommand : BaseCommandLogistic<SaveSpansCommand>
    {
        public SaveSpansCommand() : base(false, 0)
        {
        }

        public override bool CreateOrUpdate(Command command, IDictionary<string, ClientSpanEntity> clientSpanCache)
        {
            var saveSpansArgs = command.Args.As<SaveSpansArgs>();

            foreach (var item in saveSpansArgs.Items)
            {
                var clientSpanEntity = new ClientSpanEntity();
                var currentKey = item.ToLocateCurrentKey();
                MyModelHelper.SetProperties(clientSpanEntity, item);
                clientSpanCache[currentKey] = clientSpanEntity;
            }
            return true;
        }
    }

    public class StartSpanCommand : BaseCommandLogistic<StartSpanCommand>
    {
        public StartSpanCommand() : base(true, 1)
        {
        }

        public override bool CreateOrUpdate(Command command, IDictionary<string, ClientSpanEntity> clientSpanCache)
        {
            var clientSpan = (ClientSpan)command.Args;
            var currentKey = clientSpan.ToLocateCurrentKey();

            var clientSpanEntity = new ClientSpanEntity();
            MyModelHelper.SetProperties(clientSpanEntity, clientSpan);
            clientSpanEntity.StartUtc = command.CreateUtc;

            clientSpanCache[currentKey] = clientSpanEntity;
            return true;
        }
    }
    public class LogCommand : BaseCommandLogistic<LogCommand>
    {
        public LogCommand() : base(true, 2)
        {
        }
        public override bool CreateOrUpdate(Command command, IDictionary<string, ClientSpanEntity> clientSpanCache)
        {
            var logArgs = (LogArgs)command.Args;
            var currentKey = logArgs.ToLocateCurrentKey();
            if (!clientSpanCache.ContainsKey(currentKey))
            {
                return false;
            }

            var clientSpanEntity = clientSpanCache[currentKey];
            clientSpanEntity.SetLogs(logArgs.Logs);
            return true;
        }
    }
    public class SetTagCommand : BaseCommandLogistic<SetTagCommand>
    {
        public SetTagCommand() : base(true, 3)
        {
        }
        public override bool CreateOrUpdate(Command command, IDictionary<string, ClientSpanEntity> clientSpanCache)
        {
            var setTagArgs = (SetTagArgs)command.Args;
            var currentKey = setTagArgs.ToLocateCurrentKey();
            if (!clientSpanCache.ContainsKey(currentKey))
            {
                return false;
            }

            var clientSpanEntity = clientSpanCache[currentKey];
            clientSpanEntity.SetTags(setTagArgs.Tags);
            return true;
        }
    }
    public class FinishSpanCommand : BaseCommandLogistic<FinishSpanCommand>
    {
        public FinishSpanCommand() : base(true, 4)
        {
        }
        public override bool CreateOrUpdate(Command command, IDictionary<string, ClientSpanEntity> clientSpanCache)
        {
            var finishSpanArgs = (FinishSpanArgs)command.Args;
            var currentKey = finishSpanArgs.ToLocateCurrentKey();
            if (!clientSpanCache.ContainsKey(currentKey))
            {
                return false;
            }

            var clientSpanEntity = clientSpanCache[currentKey];
            clientSpanEntity.FinishUtc = command.CreateUtc;
            return true;
        }
    }

    public class CommandQueueTask
    {
        private readonly DelayedGroupCacheCommand _delayedGroupCacheCommand;

        public CommandQueueTask(DelayedGroupCacheCommand delayedGroupCacheCommand)
        {
            _delayedGroupCacheCommand = delayedGroupCacheCommand;
        }

        public IList<ClientSpanEntity> GetEntities(IList<ICommandLogistic> commandLogistics, IList<Command> allCommands, DateTime now)
        {
            var spanCache = new Dictionary<string, ClientSpanEntity>();
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

        public async Task Process(IList<IClientSpanProcess> processes, IList<ICommandLogistic> commandLogistics, CommandQueue commandQueue, DateTime now)
        {
            //process steps:
            //dequeue all commands to groupCache
            //get expired entities from commands
            //run process: save client spans
            //run process: send client spans
            //run process: ...

            var currentCommands = await commandQueue.TryDequeueAll().ConfigureAwait(false);
            var spanEntities = GetEntities(commandLogistics, currentCommands, now);

            var orderedProcesses = processes.OrderBy(x => x.SortNum).ToList();
            await Task.WhenAll(orderedProcesses.Select(x => x.Process(spanEntities)));
        }
    }

    public static class CommandExtensions
    {
        public static string GetDesc(this Command cmd)
        {
            if (cmd == null)
            {
                return null;
            }

            var argsDesc = string.Empty;
            if (cmd.Args != null)
            {
                if (cmd.Args is IClientSpanLocate clientTraceLocate)
                {
                    argsDesc = clientTraceLocate.ToDisplayKey();
                }
                else if (cmd.Args is SaveSpansArgs saveSpansArgs)
                {
                    argsDesc = "saveSpans: " + saveSpansArgs.Items.Count;
                }
            }
            return string.Format("{0} {1:yyyyMMdd-HH:mm:ss} {2}", cmd.ArgsType, cmd.CreateUtc, argsDesc);
        }

        public static string TryGetTraceId(this Command cmd)
        {
            if (cmd?.Args == null)
            {
                return null;
            }

            if (cmd.Args is IClientTraceLocate clientTraceLocate)
            {
                return clientTraceLocate.TraceId;
            }

            return null;
        }

        public static IList<IClientSpanLocate> TryGetIClientSpanLocates(this Command cmd)
        {
            if (cmd?.Args == null)
            {
                return new List<IClientSpanLocate>();
            }

            if (cmd.Args is IBatchClientSpanLocate<IClientSpanLocate> batchClientTraceLocate)
            {
                return batchClientTraceLocate.Items;
            }
            return new List<IClientSpanLocate>();
        }

        public static IEnumerable<Command> AsCommands(IList<object> jsonCommands)
        {
            foreach (var queueInfoCommand in jsonCommands)
            {
                if (queueInfoCommand is Command theCommand)
                {
                    yield return theCommand;
                }
                else
                {
                    var propName = "CommandType";
                    var tryGetProperty = queueInfoCommand.TryGetProperty(propName, true, out var propValue);
                    if (tryGetProperty)
                    {
                        yield return queueInfoCommand.As<Command>();
                    }
                }
            }
        }
    }
}