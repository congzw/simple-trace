using Microsoft.Extensions.DependencyInjection;

namespace SimpleTrace.Server.CallApis
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
