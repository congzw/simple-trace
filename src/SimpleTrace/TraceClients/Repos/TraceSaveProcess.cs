using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleTrace.TraceClients.ScheduleTasks;

namespace SimpleTrace.TraceClients.Repos
{
    public class TraceSaveProcess : IClientSpanProcess
    {
        private readonly IClientSpanRepository _clientSpanRepository;

        public TraceSaveProcess(IClientSpanRepository clientSpanRepository)
        {
            _clientSpanRepository = clientSpanRepository;
            SortNum = 1;
        }

        public float SortNum { get; set; }
        public Task Process(IList<IClientSpan> entities)
        {
            return _clientSpanRepository.Add(entities);
        }
    }
}