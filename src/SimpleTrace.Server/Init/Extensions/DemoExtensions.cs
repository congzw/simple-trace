using Microsoft.Extensions.DependencyInjection;
using SimpleTrace.Server.Demos;

namespace SimpleTrace.Server.Init.Extensions
{
    public static class DemoExtensions
    {
        public static IServiceCollection AddDemo(this IServiceCollection services)
        {
            services.AddTransient<DemoFormCtrl>();
            services.AddTransient<DemoForm>();
            return services;
        }
    }
}
