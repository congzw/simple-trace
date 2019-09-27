using SimpleTrace.Common;
using Microsoft.Extensions.DependencyInjection;
using SimpleTrace.Server.CallApis;
using SimpleTrace.TraceClients.ApiProxy;

namespace SimpleTrace.Server.Init.Extensions
{
    public static class CallApiExtensions
    {
        public static IServiceCollection AddCallApi(this IServiceCollection services)
        {
            services.AddTransient<ClientSpanRepos>();
            services.AddTransient<CallApiFormCtrl>();
            services.AddTransient<CallApiForm>();
            
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
