//using System.Collections.Generic;
//// ReSharper disable CheckNamespace

//namespace SimpleTrace.TraceClients.Commands
//{
//    public class SetTagCommand : BaseCommand
//    {
//        public SetTagCommand(SetTagArgs args) : base(args, 3)
//        {
//        }

//        public override bool CreateOrUpdate(IDictionary<string, ClientSpanEntity> clientSpanCache)
//        {
//            var setTagArgs = (SetTagArgs)this.Args;
//            var currentKey = setTagArgs.ToLocateCurrentKey();
//            if (!clientSpanCache.ContainsKey(currentKey))
//            {
//                return false;
//            }

//            var clientSpanEntity = clientSpanCache[currentKey];
//            clientSpanEntity.SetTags(setTagArgs.Tags);
//            return true;
//        }
//    }
//}