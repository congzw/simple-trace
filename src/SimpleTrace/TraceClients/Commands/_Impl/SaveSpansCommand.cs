﻿using System.Collections.Generic;
using SimpleTrace.Common;
// ReSharper disable CheckNamespace

namespace SimpleTrace.TraceClients.Commands
{
    public class SaveSpansCommand : BaseCommandLogistic<SaveSpansCommand>
    {
        public SaveSpansCommand() : base(false, 0)
        {
        }

        public override bool CreateOrUpdate(Command command, IDictionary<string, IClientSpan> clientSpanCache)
        {
            var saveSpansArgs = command.Args.FromJTokenOrObject<SaveSpansArgs>();

            foreach (var item in saveSpansArgs.Items)
            {
                var currentKey = item.ToLocateCurrentKey();
                clientSpanCache[currentKey] = item;
            }
            return true;
        }
    }
}