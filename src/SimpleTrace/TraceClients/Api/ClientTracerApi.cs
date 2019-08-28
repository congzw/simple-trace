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
            return _commandQueue.Enqueue(new StartSpanCommand(args));
        }

        public Task Log(LogArgs args)
        {
            if (args.IsBadLocateArgs(ClientSpanLocateMode.ForCurrent))
            {
                return Task.FromResult(0);
            }
            return _commandQueue.Enqueue(new LogCommand(args));
        }

        public Task SetTags(SetTagArgs args)
        {
            if (args.IsBadLocateArgs(ClientSpanLocateMode.ForCurrent))
            {
                return Task.FromResult(0);
            }
            return _commandQueue.Enqueue(new SetTagCommand(args));
        }

        public Task FinishSpan(FinishSpanArgs args)
        {
            if (args.IsBadLocateArgs(ClientSpanLocateMode.ForCurrent))
            {
                return Task.FromResult(0);
            }
            return _commandQueue.Enqueue(new FinishedCommand(args));
        }

        public Task SaveSpans(SaveSpansArgs args)
        {
            return _commandQueue.Enqueue(new SaveSpansCommand(args));
        }

        public Task<QueueInfo> GetQueueInfo(GetQueueInfoArgs args)
        {
            var commandQueueInfo = new QueueInfo();
            var commands = _commandQueue.Items.ToArray();
            commandQueueInfo.TotalCount = commands.Length;
            commandQueueInfo.Commands = commands;
            return Task.FromResult(commandQueueInfo);
        }
    }
}
