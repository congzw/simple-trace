using System.Collections.Generic;
using Common;

// ReSharper disable CheckNamespace

namespace SimpleTrace.TraceClients.Commands
{
    public class FinishSpanCommand : BaseCommandLogistic<FinishSpanCommand>
    {
        public FinishSpanCommand() : base(true, 4)
        {
        }
        public override bool CreateOrUpdate(Command command, IDictionary<string, ClientSpanEntity> clientSpanCache)
        {
            var finishSpanArgs = command.Args.As<FinishSpanArgs>();
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
}