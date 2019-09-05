using Common;
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
            AddCommon(services);
            AddTraceClients(services);
        }

        public static void ConfigSimpleTrace(this IApplicationBuilder app, IHostingEnvironment env, IConfiguration configuration)
        {
        }

        private static void AddCommon(IServiceCollection services)
        {
            services.AddSingleton(SimpleJson.Resolve() as SimpleJson);
            services.AddSingleton(SimpleJson.Resolve());
            services.AddSingleton(SimpleJson.ResolveSimpleJsonFile());
            
            services.AddSingleton(SimpleLogFactory.Resolve() as SimpleLogFactory);
            services.AddSingleton(SimpleLogFactory.Resolve());

            services.AddSingleton(WebApiHelper.Resolve() as WebApiHelper);
            services.AddSingleton(WebApiHelper.Resolve());

            //also can use like this: 
            //services.AddSingleton<WebApiHelper>();
            //services.AddSingleton<IWebApiHelper>(sp => sp.GetService<WebApiHelper>());
            //WebApiHelper.Resolve = () => app.ApplicationServices.GetService<IWebApiHelper>(); //in Config process
        }
        private static void AddTraceClients(IServiceCollection services)
        {
            services.AddSingleton<IClientTracerApi, ClientTracerApi>();
            services.AddSingleton<CommandQueue>();
        }
    }
}