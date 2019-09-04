using System;
using System.Threading.Tasks;
using Common;
using SimpleTrace.TraceClients.Api;
using SimpleTrace.TraceClients.Commands;

namespace SimpleTrace.TraceClients.ApiProxy
{
    //direct to open trace, can be used for single instance client
    public class LocalClientTracerApiProxy : IClientTracerApiProxy
    {
        private readonly ClientTracerApi _clientTracerApi;

        public LocalClientTracerApiProxy(ClientTracerApi clientTracerApi)
        {
            _clientTracerApi = clientTracerApi;
        }

        public Task StartSpan(ClientSpan args)
        {
            return _clientTracerApi.StartSpan(args);
        }

        public Task Log(LogArgs args)
        {
            return _clientTracerApi.Log(args);
        }

        public Task SetTags(SetTagArgs args)
        {
            return _clientTracerApi.SetTags(args);
        }

        public Task FinishSpan(FinishSpanArgs args)
        {
            return _clientTracerApi.FinishSpan(args);
        }

        public Task SaveSpans(SaveSpansArgs args)
        {
            return _clientTracerApi.SaveSpans(args);
        }

        public Task<QueueInfo> GetQueueInfo(GetQueueInfoArgs args)
        {
            return _clientTracerApi.GetQueueInfo(args);
        }

        public Task<DateTime> GetDate()
        {
            return DateHelper.Instance.GetDateNow().AsTask();
        }

        public Task<bool> TryTestApiConnection()
        {
            return true.AsTask();
        }
    }

    public class LocalClientTracerApiProxyConfig
    {
        public static void Setup()
        {
            //services.AddSingleton<IClientTracerApi, ClientTracerApi>();
            //services.AddSingleton<CommandQueue>();

            var clientTracerApiProxy = new LocalClientTracerApiProxy(new ClientTracerApi(new CommandQueue()));
            ApiProxyInit.Reset(clientTracerApiProxy, null, null);
            //todo CommandQueueTask Config
        }
    }
}
