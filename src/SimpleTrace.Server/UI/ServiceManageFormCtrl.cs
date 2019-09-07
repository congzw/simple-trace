using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using SimpleTrace.TraceClients;
using SimpleTrace.TraceClients.ApiProxy;
using SimpleTrace.TraceClients.Repos;

namespace SimpleTrace.Server.UI
{
    public class ServiceManageFormCtrl
    {
        private readonly IClientSpanRepository _clientSpanRepository;
        private readonly IClientTracerApiProxy _apiProxy;

        public ServiceManageFormCtrl(IClientSpanRepository clientSpanRepository, IClientTracerApiProxy apiProxy)
        {
            _clientSpanRepository = clientSpanRepository;
            _apiProxy = apiProxy;
        }

        public Task<IList<IClientSpan>> ReadClientSpanEntities()
        {
            return _clientSpanRepository.Read(null);
        }

        public Task SendApiSpans(IList<IClientSpan> clientSpans)
        {
            var args = SaveSpansArgs.Create(clientSpans.ToArray());

            var vr = SaveSpansArgs.Validate(args);
            if (!vr.Success)
            {
                LogInfo(vr.Message + " => " + vr.Data.ToJson(false));
                return Task.FromResult(0);
            }

            return _apiProxy.SaveSpans(args);
        }

        private void LogInfo(string info)
        {
            var logger = SimpleLogSingleton<ServiceManageFormCtrl>.Instance.Logger;
            logger.LogInfo(info);
        }
    }
}
