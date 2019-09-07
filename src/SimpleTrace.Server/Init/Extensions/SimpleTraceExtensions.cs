using Common;
using Microsoft.Extensions.DependencyInjection;
using SimpleTrace.TraceClients.ApiProxy;
using SimpleTrace.TraceClients.Repos;

namespace SimpleTrace.Server.Init.Extensions
{
    public static class SimpleTraceExtensions
    {
        public static IServiceCollection AddSimpleTrace(this IServiceCollection services)
        {
            services.AddSingleton<IClientSpanRepository, ClientSpanRepository>();
            
            //todo read from config
            var apiProxyConfig = new ApiProxyConfig();
            services.AddSingleton(apiProxyConfig);

            //IClientTracerApiProxy
            services.AddSingleton<IClientTracerApiProxy>(sp =>
            {
                var webApiHelper = sp.GetService<IWebApiHelper>();
                var proxyConfig = sp.GetService<ApiProxyConfig>();
                var httpClientTracerApiProxy = new HttpClientTracerApiProxy(webApiHelper, proxyConfig);
                var apiProxySmartWrapper = ClientTracerApiProxySmartWrapper.Resolve();
                apiProxySmartWrapper.Reset(httpClientTracerApiProxy);

                return apiProxySmartWrapper;
            });

            return services;
        }
    }
}
