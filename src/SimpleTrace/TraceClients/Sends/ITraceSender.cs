using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleTrace.TraceClients.Sends
{
    public interface ITraceSender
    {
        Task Send(IList<ClientSpanEntity> entities);
    }
}
