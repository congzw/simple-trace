using Microsoft.Extensions.DependencyInjection;
using SimpleTrace.Server.UI;

namespace SimpleTrace.Server.Init.Extensions
{
    public static class DemoFormExtensions
    {
        public static IServiceCollection AddDemoForm(this IServiceCollection services)
        {
            services.AddTransient<DemoFormCtrl>();
            services.AddTransient<DemoForm>();
            return services;
        }
    }
}
