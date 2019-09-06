using System;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleTrace.Server.Init
{
    public class MyContainer
    {
        protected IServiceCollection Services { get; set; }

        protected ServiceProvider ServiceProvider { get; set; }

        public void Init(Action<IServiceCollection> configureServices)
        {
            if (Services != null)
            {
                return;
            }

            Services = new ServiceCollection();
            configureServices(Services);
            ServiceProvider = Services.BuildServiceProvider();
        }
        
        public T GetService<T>()
        {
            return ServiceProvider.GetService<T>();
        }

        public static MyContainer Instance = new MyContainer();
    }
}
