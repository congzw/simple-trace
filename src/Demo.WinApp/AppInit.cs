using SimpleTrace.Common;
using SimpleTrace.OpenTrace;
using SimpleTrace.OpenTrace.Jaeger;
using SimpleTrace.TraceClients.ApiProxy;

namespace Demo.WinApp
{
    public class AppInit
    {
        public static void Init()
        {
            SetupAsyncLog();
            SetupTraceApi();
            SetupJaeger();
        }

        private static void SetupJaeger()
        {
            var jaegerTracerConfig = new JaegerTracerConfig();
            jaegerTracerConfig.DefaultTracerId = "Default-Tracer";
            jaegerTracerConfig.TraceEndPoint = "http://localhost:14268/api/traces";
            var tracerFactory = new JaegerTracerFactory(jaegerTracerConfig);

            //replace null
            var tracerContext = new TracerContext(tracerFactory);
            TracerContext.Resolve = () => tracerContext;
        }

        private static void SetupTraceApi()
        {
            //with api
            //todo read from config
            var apiProxyConfig = new ApiProxyConfig();
            //apiProxyConfig.BaseUri = "http://localhost:16685/api/trace";
            apiProxyConfig.BaseUri = "http://192.168.1.133:16685/api/trace";
            apiProxyConfig.FailTimeoutMilliseconds = 200;
            var webApiHelper = WebApiHelper.Resolve();

            var httpClientTracerApiProxy = new HttpClientTracerApiProxy(webApiHelper, apiProxyConfig);
            ApiProxyInit.Reset(httpClientTracerApiProxy, null, null);

            //var simpleIoc = SimpleIoc.Instance;

            //var knownCommands = KnownCommands.Instance;
            //knownCommands.Setup();
            //simpleIoc.Register(() => knownCommands);

            //var commandQueue = new CommandQueue();
            //simpleIoc.Register(() => commandQueue);

            //var commandQueueTask = new CommandQueueTask();
            //simpleIoc.Register(() => commandQueueTask);

            //var clientTracerApi = new ClientTracerApi(commandQueue);
            //simpleIoc.Register(() => clientTracerApi);
            //simpleIoc.Register(() =>
            //{
            //    var clientSpanProcesses = new List<IClientSpanProcess>();
            //    clientSpanProcesses.Add(new TraceSendProcess(new JaegerTraceSender()));
            //    clientSpanProcesses.Add(new TraceSaveProcess(new ClientSpanRepository(AsyncFile.Instance)));
            //    return clientSpanProcesses;
            //});

            ////without api
            //LocalClientTracerApiProxyConfig.Setup(clientTracerApi);
        }

        private static ISimpleLogFactory SetupAsyncLog()
        {
            var simpleLogFactory = SimpleLogFactory.Resolve();
            simpleLogFactory.LogWithSimpleEventBus();
            return simpleLogFactory;
        }
    }
}
