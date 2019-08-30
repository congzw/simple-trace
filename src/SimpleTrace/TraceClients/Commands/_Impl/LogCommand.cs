//using System.Collections.Generic;
//// ReSharper disable CheckNamespace

//namespace SimpleTrace.TraceClients.Commands
//{
//    public class LogCommand : BaseCommand
//    {
//        public LogCommand(LogArgs args) : base(args, 2)
//        {
//        }

//        public override bool CreateOrUpdate(IDictionary<string, ClientSpanEntity> clientSpanCache)
//        {
//            var logArgs = (LogArgs)this.Args;
//            var currentKey = logArgs.ToLocateCurrentKey();
//            if (!clientSpanCache.ContainsKey(currentKey))
//            {
//                return false;
//            }

//            var clientSpanEntity = clientSpanCache[currentKey];
//            clientSpanEntity.SetLogs(logArgs.Logs);
//            return true;
//        }
//    }
//}