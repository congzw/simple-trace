using Microsoft.Extensions.DependencyInjection;
using SimpleTrace.Server.UI;

namespace SimpleTrace.Server.Init.Extensions
{
    public static class ServiceManageExtensions
    {
        public static IServiceCollection AddServiceManage(this IServiceCollection services)
        {
            services.AddTransient<ServiceManageFormCtrl>();
            services.AddTransient<ServiceManageForm>();
            return services;
        }
    }
}
