using Microsoft.Extensions.DependencyInjection;
using SimpleTrace.Server.Init.Extensions;

namespace SimpleTrace.Server.Init
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCommon();
            services.AddServicesForm();
            services.AddSimpleTrace();
        }
    }
}
