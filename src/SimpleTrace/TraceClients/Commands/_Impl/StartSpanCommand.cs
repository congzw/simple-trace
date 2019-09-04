using System.Collections.Generic;
using Common;

// ReSharper disable CheckNamespace

namespace SimpleTrace.TraceClients.Commands
{
    public class StartSpanCommand : BaseCommandLogistic<StartSpanCommand>
    {
        public StartSpanCommand() : base(true, 1)
        {
        }

        public override bool CreateOrUpdate(Command command, IDictionary<string, IClientSpan> clientSpanCache)
        {
            var clientSpan = command.Args.As<ClientSpan>();
            var currentKey = clientSpan.ToLocateCurrentKey();

            //var clientSpanEntity = new ClientSpanEntity();
            //MyModelHelper.SetProperties(clientSpanEntity, clientSpan);
            //clientSpanEntity.StartUtc = command.CreateUtc;

            clientSpan.StartUtc = command.CreateUtc;
            clientSpanCache[currentKey] = clientSpan;
            return true;
        }
    }
}