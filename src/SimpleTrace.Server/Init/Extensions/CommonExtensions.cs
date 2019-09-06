using Common;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleTrace.Server.Init.Extensions
{
    public static class CommonExtensions
    {
        public static IServiceCollection AddCommon(this IServiceCollection services)
        {
            services.AddSingleton(sp =>
            {
                var simpleLogFactory = SimpleLogFactory.Resolve();
                simpleLogFactory.LogWithSimpleEventBus();
                return simpleLogFactory;
            });
            return services;
        }
    }
}