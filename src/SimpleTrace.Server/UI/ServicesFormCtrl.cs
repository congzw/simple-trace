using System;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SimpleTrace.Server.Demo;

namespace SimpleTrace.Server.UI
{
    public class ServicesFormCtrl
    {
        public void ShowInfos(ServiceLifetime lifetime)
        {
            var objectCounter = ObjectCounter.Instance;
            objectCounter.Enabled = true;
            var services = new ServiceCollection();

            services.Add(ServiceDescriptor.Describe(typeof(IFooService), typeof(FooService), lifetime));
            services.Add(ServiceDescriptor.Describe(typeof(BarService), typeof(BarService), lifetime));

            var simpleLogFactory = SimpleLogFactory.Resolve();
            simpleLogFactory.LogWithSimpleEventBus();
            services.AddSingleton(simpleLogFactory);
            
            using (var sp = services.BuildServiceProvider())
            {
                for (int i = 0; i < 2; i++)
                {
                    var barService = sp.GetService<BarService>();
                    Task.Delay(TimeSpan.FromMilliseconds(200)).Wait();
                }
            }
        }
    }
}
