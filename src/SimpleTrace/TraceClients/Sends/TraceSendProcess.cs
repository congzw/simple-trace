using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleTrace.TraceClients.ScheduleTasks;

namespace SimpleTrace.TraceClients.Sends
{
    public class TraceSendProcess : IClientSpanProcess
    {
        private readonly ITraceSender _traceSender;

        public TraceSendProcess(ITraceSender traceSender)
        {
            _traceSender = traceSender;
            SortNum = 2;
        }

        public float SortNum { get; set; }

        public Task Process(IList<IClientSpan> entities)
        {
            return _traceSender.Send(entities);
        }
    }
}
