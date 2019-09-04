using System.Collections.Generic;
using Common;

// ReSharper disable CheckNamespace

namespace SimpleTrace.TraceClients.Commands
{
    public class LogCommand : BaseCommandLogistic<LogCommand>
    {
        public LogCommand() : base(true, 2)
        {
        }
        public override bool CreateOrUpdate(Command command, IDictionary<string, IClientSpan> clientSpanCache)
        {
            var logArgs = command.Args.FromJTokenOrObject<LogArgs>();
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
}