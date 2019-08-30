using System;
using System.Collections.Generic;
using SimpleTrace.Common;

// ReSharper disable CheckNamespace

namespace SimpleTrace.TraceClients.Commands
{
    public class SaveSpansCommand : BaseCommand
    {
        public SaveSpansCommand(SaveSpansArgs args) : base(args, 0)
        {
        }

        public override bool CreateOrUpdate(IDictionary<string, ClientSpanEntity> clientSpanCache)
        {
            var saveSpansArgs = (SaveSpansArgs)this.Args;

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