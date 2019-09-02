using System.Collections.Generic;
using Common;
// ReSharper disable CheckNamespace

namespace SimpleTrace.TraceClients.Commands
{
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
}