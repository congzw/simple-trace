using Microsoft.Extensions.DependencyInjection;
using SimpleTrace.Server.UI;

namespace SimpleTrace.Server.Demo
{
    public static class FooExtensions
    {
        public static IServiceCollection AddFoo(this IServiceCollection services)
        {
            //todo
            services.AddTransient<ServicesFormCtrl>();
            services.AddTransient<ServicesForm>();
            return services;
        }
    }
}
