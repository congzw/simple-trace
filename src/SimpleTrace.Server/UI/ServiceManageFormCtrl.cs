using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleTrace.TraceClients;
using SimpleTrace.TraceClients.Repos;

namespace SimpleTrace.Server.UI
{
    public class ServiceManageFormCtrl
    {
        private readonly IClientSpanRepository _clientSpanRepository;

        public ServiceManageFormCtrl(IClientSpanRepository clientSpanRepository)
        {
            _clientSpanRepository = clientSpanRepository;
        }

        public Task<IList<IClientSpan>> ReadClientSpanEntities()
        {
            return _clientSpanRepository.Read(null);
        }
    }
}
