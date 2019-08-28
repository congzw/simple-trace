using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleTrace.TraceClients.Sends
{
    public class EmptyTraceSender : ITraceSender
    {
        public Task Send(IList<ClientSpanEntity> entities)
        {
            return Task.FromResult(0);
        }
    }
}