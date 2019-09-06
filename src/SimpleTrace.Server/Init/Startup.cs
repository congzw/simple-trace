using Microsoft.Extensions.DependencyInjection;
using SimpleTrace.Server.Demo;
using SimpleTrace.Server.Init.Extensions;

namespace SimpleTrace.Server.Init
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCommon();
            services.AddFoo();
            services.AddSimpleTrace();
        }
    }
}
