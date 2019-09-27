using SimpleTrace.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleTrace.Api.Extensions
{
    public static class CommonExtensions
    {
        public static void AddCommon(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(SimpleJson.Resolve() as SimpleJson);
            services.AddSingleton(SimpleJson.Resolve());
            services.AddSingleton(SimpleJson.ResolveSimpleJsonFile());

            services.AddSingleton(SimpleLogFactory.Resolve() as SimpleLogFactory);
            services.AddSingleton(SimpleLogFactory.Resolve());

            services.AddSingleton(WebApiHelper.Resolve() as WebApiHelper);
            services.AddSingleton(WebApiHelper.Resolve());

            services.AddSingleton(AsyncFile.Instance);

            //also can use like this: 
            //services.AddSingleton<WebApiHelper>();
            //services.AddSingleton<IWebApiHelper>(sp => sp.GetService<WebApiHelper>());
            //WebApiHelper.Resolve = () => app.ApplicationServices.GetService<IWebApiHelper>(); //in Config process
        }
    }
}
