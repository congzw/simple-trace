using System.Collections.Generic;
using SimpleTrace.Common;

// ReSharper disable CheckNamespace

namespace SimpleTrace.TraceClients.Commands
{
    public class LogCommand : BaseCommandLogistic<LogCommand>
    {
        public LogCommand() : base(true, 2)
        {
        }
        public override bool CreateOrUpdate(Command command, IDictionary<string, ClientSpanEntity> clientSpanCache)
        {
            var logArgs = command.Args.As<LogArgs>();
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