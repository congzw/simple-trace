using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleTrace.TraceClients.Api;
using SimpleTrace.TraceClients.Commands;

namespace SimpleTrace.Api.Extensions
{
    public static class SimpleTraceExtensions
    {
        public static void AddSimpleTrace(this IServiceCollection services)
        {
            services.AddSingleton<IClientTracerApi, ClientTracerApi>();
            services.AddSingleton<CommandQueue>();
        }

        public static void ConfigSimpleTrace(this IApplicationBuilder app, IHostingEnvironment env, IConfiguration configuration)
        {
        }
    }
}