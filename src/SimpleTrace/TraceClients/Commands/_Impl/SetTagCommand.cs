using System.Collections.Generic;
using Common;

// ReSharper disable CheckNamespace

namespace SimpleTrace.TraceClients.Commands
{

    public class SetTagCommand : BaseCommandLogistic<SetTagCommand>
    {
        public SetTagCommand() : base(true, 3)
        {
        }
        public override bool CreateOrUpdate(Command command, IDictionary<string, ClientSpanEntity> clientSpanCache)
        {
            var setTagArgs = command.Args.As<SetTagArgs>();
            var currentKey = setTagArgs.ToLocateCurrentKey();
            if (!clientSpanCache.ContainsKey(currentKey))
            {
                return false;
            }

            var clientSpanEntity = clientSpanCache[currentKey];
            clientSpanEntity.SetTags(setTagArgs.Tags);
            return true;
        }
    }
}