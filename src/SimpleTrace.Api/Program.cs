using System;
using SimpleTrace.Common;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace SimpleTrace.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(cfg => 
                {
                    //fix windows service base path system32 bugs!
                    SimpleLogSingleton<Program>.Instance.Logger.LogInfo("BaseDirectory: " + AppDomain.CurrentDomain.BaseDirectory);
                    cfg.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
                })
            .UseStartup<Startup>();
    }
}
