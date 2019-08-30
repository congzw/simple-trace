using System;
using System.Collections.Generic;
using SimpleTrace.Common;

namespace SimpleTrace.TraceClients.Commands.Logics
{
    public class Command
    {
        protected Command()
        {
            CreateUtc = DateHelper.Instance.GetDateNow();
        }

        public float ProcessSort { get; set; }
        public string CommandType { get; set; }
        public DateTime CreateUtc { get; set; }
        public object Args { get; set; }
    }
    
    public interface ICommandLogic
    {
        bool CreateOrUpdate(Command command, IDictionary<string, ClientSpanEntity> clientSpanCache);
    }

    public class StartSpanCommand : ICommandLogic
    {
        public bool CreateOrUpdate(Command command, IDictionary<string, ClientSpanEntity> clientSpanCache)
        {
            var clientSpan = (ClientSpan)command.Args;
            var currentKey = clientSpan.ToLocateCurrentKey();

            var clientSpanEntity = new ClientSpanEntity();
            clientSpanEntity.With(clientSpan);
            clientSpanEntity.SetBags(clientSpan.Bags);

            clientSpanCache[currentKey] = clientSpanEntity;
            return true;
        }
    }
    public class SaveSpansCommand : ICommandLogic
    {
        public bool CreateOrUpdate(Command command, IDictionary<string, ClientSpanEntity> clientSpanCache)
        {
            var saveSpansArgs = (SaveSpansArgs)command.Args;

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