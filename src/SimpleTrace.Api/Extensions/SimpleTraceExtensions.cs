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
            SetupCommon(services);

            services.AddSingleton<IClientTracerApi, ClientTracerApi>();
            services.AddSingleton<CommandQueue>();
        }

        public static void ConfigSimpleTrace(this IApplicationBuilder app, IHostingEnvironment env, IConfiguration configuration)
        {
            ConfigCommon(app, env, configuration);
        }

        private static void SetupCommon(IServiceCollection services)
        {
            services.AddSingleton<SimpleJson>();
            services.AddSingleton<ISimpleJsonFile>(sp => sp.GetService<SimpleJson>());
            services.AddSingleton<ISimpleJson>(sp => sp.GetService<SimpleJson>());

            //also can use like this: 
            //services.AddSingleton<SimpleLogFactory>();
            //services.AddSingleton<ISimpleLogFactory>(sp => sp.GetService<SimpleLogFactory>());
            //SimpleLogFactory.Resolve = () => app.ApplicationServices.GetService<ISimpleLogFactory>();
            services.AddSingleton(SimpleLogFactory.Resolve());
            services.AddSingleton<SimpleLogFactory>(SimpleLogFactory.Resolve() as SimpleLogFactory);

            services.AddSingleton<WebApiHelper>();
            services.AddSingleton<IWebApiHelper>(sp => sp.GetService<WebApiHelper>());
        }
        private static void ConfigCommon(this IApplicationBuilder app, IHostingEnvironment env, IConfiguration configuration)
        {
            SimpleJson.Resolve = () => app.ApplicationServices.GetService<ISimpleJson>();
            SimpleJson.ResolveSimpleJsonFile = () => app.ApplicationServices.GetService<ISimpleJsonFile>();
            WebApiHelper.Resolve = () => app.ApplicationServices.GetService<IWebApiHelper>();
        }
    }
}