using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleTrace.TraceClients;
using SimpleTrace.TraceClients.Sends;

namespace SimpleTrace.OpenTrace.Jaeger
{
    public class TraceSender : ITraceSender
    {
        public Task Send(IList<ClientSpanEntity> entities)
        {
            throw new System.NotImplementedException();
        }
    }
}
