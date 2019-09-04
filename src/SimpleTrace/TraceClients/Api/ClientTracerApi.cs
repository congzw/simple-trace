﻿using System.Linq;
using System.Threading.Tasks;
using SimpleTrace.TraceClients.Commands;

namespace SimpleTrace.TraceClients.Api
{
    public class ClientTracerApi : IClientTracerApi
    {
        private readonly CommandQueue _commandQueue;

        public ClientTracerApi(CommandQueue commandQueue)
        {
            _commandQueue = commandQueue;
        }

        public Task StartSpan(ClientSpan args)
        {
            if (!args.ValidateNewClientSpan())
            {
                return Task.FromResult(0);
            }
            return _commandQueue.Enqueue(StartSpanCommand.Create(args));
        }

        public Task Log(LogArgs args)
        {
            if (args.IsBadLocateArgs(ClientSpanLocateMode.ForCurrent))
            {
                return Task.FromResult(0);
            }
            return _commandQueue.Enqueue(LogCommand.Create(args));
        }

        public Task SetTags(SetTagArgs args)
        {
            if (args.IsBadLocateArgs(ClientSpanLocateMode.ForCurrent))
            {
                return Task.FromResult(0);
            }
            return _commandQueue.Enqueue(SetTagCommand.Create(args));
        }

        public Task FinishSpan(FinishSpanArgs args)
        {
            if (args.IsBadLocateArgs(ClientSpanLocateMode.ForCurrent))
            {
                return Task.FromResult(0);
            }
            return _commandQueue.Enqueue(FinishSpanCommand.Create(args));
        }

        public Task SaveSpans(SaveSpansArgs args)
        {
            return _commandQueue.Enqueue(SaveSpansCommand.Create(args));
        }

        public Task<QueueInfo> GetQueueInfo(GetQueueInfoArgs args)
        {
            var queueInfo = new QueueInfo();
            var commands = _commandQueue.Items.ToList();
            queueInfo.TotalCount = commands.Count;

            if (args.IsTrue(args.WithCommands))
            {
                //(IList<object>)xxx => InvalidCastException: 'IList<ICommand>' to type 'IList<Object>'.
                queueInfo.Commands = commands.Cast<object>().ToList();
            }

            if (args.IsTrue(args.WithCommandSums))
            {
                var commandSums = commands.GroupBy(x => x.CommandType).Select(g =>
                {
                    var commandSum = new CommandSum();
                    commandSum.CommandType = g.Key;
                    commandSum.CommandCount = g.Count();
                    return commandSum;
                }).ToList();

                queueInfo.CommandSums = commandSums;
            }
            return Task.FromResult(queueInfo);
        }
    }
}
