using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleTrace.TraceClients.Sends
{
    public class NullTraceSender : ITraceSender
    {
        public Task Send(IList<ClientSpanEntity> entities)
        {
            return Task.FromResult(0);
        }
    }
}