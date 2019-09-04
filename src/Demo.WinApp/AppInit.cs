﻿using Common;
using SimpleTrace.OpenTrace;
using SimpleTrace.OpenTrace.Jaeger;
using SimpleTrace.TraceClients.ApiProxy;
using SimpleTrace.TraceClients.Commands;

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
            ////with api
            ////todo read from config
            //var apiProxyConfig = new ApiProxyConfig();
            //apiProxyConfig.BaseUri = "http://localhost:16685/api/trace";
            //apiProxyConfig.FailTimeoutMilliseconds = 200;
            //var webApiHelper = WebApiHelper.Resolve();

            //var httpClientTracerApiProxy = new HttpClientTracerApiProxy(webApiHelper, apiProxyConfig);
            //ApiProxyInit.Reset(httpClientTracerApiProxy, null, null);
            //without api
            LocalClientTracerApiProxyConfig.Setup();

            var knownCommands = KnownCommands.Instance;
            knownCommands.Setup();
        }

        private static ISimpleLogFactory SetupAsyncLog()
        {
            var simpleLogFactory = SimpleLogFactory.Resolve();
            simpleLogFactory.LogWithSimpleEventBus();
            return simpleLogFactory;
        }
    }
}
