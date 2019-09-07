using Microsoft.Extensions.DependencyInjection;
using SimpleTrace.Server.CallApis;

namespace SimpleTrace.Server.Init.Extensions
{
    public static class CallApiExtensions
    {
        public static IServiceCollection AddCallApi(this IServiceCollection services)
        {
            services.AddTransient<CallApiFormCtrl>();
            services.AddTransient<CallApiForm>();
            return services;
        }
    }
}
