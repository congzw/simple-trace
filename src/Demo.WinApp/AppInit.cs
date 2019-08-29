using Common;
using SimpleTrace.Common;
using SimpleTrace.TraceClients.ApiProxy;

namespace Demo.WinApp
{
    public class AppInit
    {
        public static void Init()
        {
            SetupAsyncLog();

            //todo read from config
            var apiProxyConfig = new ApiProxyConfig();
            apiProxyConfig.BaseUri = "http://localhost:16685/api/trace";
            apiProxyConfig.FailTimeoutMilliseconds = 200;
            var webApiHelper = WebApiHelper.Resolve();

            var httpClientTracerApiProxy = new HttpClientTracerApiProxy(webApiHelper, apiProxyConfig);
            ApiProxyInit.Reset(httpClientTracerApiProxy, null, null);
        }
        
        private static ISimpleLogFactory SetupAsyncLog()
        {
            var simpleLogFactory = SimpleLogFactory.Resolve();
            simpleLogFactory.LogWithSimpleEventBus();
            return simpleLogFactory;
        }
    }
}
