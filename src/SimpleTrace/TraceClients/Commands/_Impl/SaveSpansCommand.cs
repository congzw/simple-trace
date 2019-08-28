using System;
using System.Collections.Generic;
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
            throw new NotImplementedException();
        }
    }
}