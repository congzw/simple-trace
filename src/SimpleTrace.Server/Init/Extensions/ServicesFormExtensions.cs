using Microsoft.Extensions.DependencyInjection;
using SimpleTrace.Server.UI;

namespace SimpleTrace.Server.Init.Extensions
{
    public static class ServicesFormExtensions
    {
        public static IServiceCollection AddServicesForm(this IServiceCollection services)
        {
            services.AddTransient<ServicesFormCtrl>();
            services.AddTransient<ServicesForm>();
            return services;
        }
    }
}
