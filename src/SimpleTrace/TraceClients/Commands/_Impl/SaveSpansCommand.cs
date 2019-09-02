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
                var clientSpanEntity = Create(item);
                var currentKey = item.ToLocateCurrentKey();
                clientSpanCache[currentKey] = clientSpanEntity;
            }
            return true;
        }

        private static ClientSpanEntity Create(SaveClientSpan saveClientSpan)
        {
            var clientSpanEntity = new ClientSpanEntity();
            MyModelHelper.SetProperties(clientSpanEntity, saveClientSpan, new[] { "Logs" });

            foreach (var log in saveClientSpan.Logs)
            {
                var keyValueInfo = KeyValueInfo.Create(log, saveClientSpan.StartUtc);
                clientSpanEntity.Logs[keyValueInfo.KeyValuePair.Key] = keyValueInfo;
            }

            return clientSpanEntity;
        }
    }
}