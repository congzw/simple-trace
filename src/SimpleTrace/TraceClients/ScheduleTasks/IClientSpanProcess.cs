using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleTrace.TraceClients.ScheduleTasks
{
    public interface IClientSpanProcess
    {
        float SortNum { get; set; }
        Task Process(IList<ClientSpanEntity> entities);
    }
}