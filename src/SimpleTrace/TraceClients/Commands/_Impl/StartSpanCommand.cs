using System.Collections.Generic;
// ReSharper disable CheckNamespace

namespace SimpleTrace.TraceClients.Commands
{
    public class StartSpanCommand : BaseCommand
    {
        public StartSpanCommand(ClientSpan args) : base(args, 1)
        {
        }

        public override bool CreateOrUpdate(IDictionary<string, ClientSpanEntity> clientSpanCache)
        {
            var clientSpan = (ClientSpan)this.Args;
            var currentKey = clientSpan.ToLocateCurrentKey();

            var clientSpanEntity = new ClientSpanEntity();
            clientSpanEntity.With(clientSpan);
            clientSpanEntity.SetBags(clientSpan.Bags);

            clientSpanCache[currentKey] = clientSpanEntity;
            return true;
        }
    }
}