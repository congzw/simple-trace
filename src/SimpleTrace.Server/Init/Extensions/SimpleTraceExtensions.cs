using Microsoft.Extensions.DependencyInjection;
using SimpleTrace.Server.ServiceManages;

namespace SimpleTrace.Server.Init.Extensions
{
    public static class SimpleTraceExtensions
    {
        public static IServiceCollection AddSimpleTrace(this IServiceCollection services)
        {
            services.AddTransient<ServiceManageFormCtrl>();
            services.AddTransient<ServiceManageForm>();
            return services;
        }
    }
}
