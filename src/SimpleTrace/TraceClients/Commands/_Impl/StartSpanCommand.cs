using System.Collections.Generic;
using SimpleTrace.Common;

// ReSharper disable CheckNamespace

namespace SimpleTrace.TraceClients.Commands
{
    public class StartSpanCommand : BaseCommandLogistic<StartSpanCommand>
    {
        public StartSpanCommand() : base(true, 1)
        {
        }

        public override bool CreateOrUpdate(Command command, IDictionary<string, ClientSpanEntity> clientSpanCache)
        {
            var clientSpan = command.Args.As<ClientSpan>();
            var currentKey = clientSpan.ToLocateCurrentKey();

            var clientSpanEntity = new ClientSpanEntity();
            MyModelHelper.SetProperties(clientSpanEntity, clientSpan);
            clientSpanEntity.StartUtc = command.CreateUtc;

            clientSpanCache[currentKey] = clientSpanEntity;
            return true;
        }
    }
}