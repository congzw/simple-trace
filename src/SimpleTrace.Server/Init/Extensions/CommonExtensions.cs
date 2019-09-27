using System;
using System.IO;
using SimpleTrace.Common;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleTrace.Server.Init.Extensions
{
    public static class CommonExtensions
    {
        public static IServiceCollection AddCommon(this IServiceCollection services)
        {
            services.AddSingleton(AsyncFile.Instance);

            var simpleLogFactory = SetupLog();
            services.AddSingleton(sp => simpleLogFactory);

            var webApiHelper = WebApiHelper.Resolve();
            services.AddSingleton(webApiHelper);

            return services;
        }

        private static ISimpleLogFactory SetupLog()
        {
            var simpleLogFactory = SimpleLogFactory.Resolve();
            simpleLogFactory.LogWithSimpleEventBus();

            var initLog = simpleLogFactory.GetOrCreate(null);
            _folderPath = AppDomain.CurrentDomain.Combine("Logs");
            initLog.LogInfo(">>>> log folder path => " + _folderPath);

            var logActions = simpleLogFactory.LogActions;
            logActions["LogExToFile"] = new LogMessageAction("LogExToFile", true, args =>
            {
                if (args.Level.ShouldLog(SimpleLogLevel.Error))
                {
                    LogExToFile(args);
                }
            });

            return simpleLogFactory;
        }

        private static string _folderPath = null;
        private static void LogExToFile(LogMessageArgs args)
        {
            try
            {
                var level = args.Level;
                var message = args.Message;
                if (level >= SimpleLogLevel.Error && level < SimpleLogLevel.None)
                {
                    var now = DateHelper.Instance.GetDateNow();
                    var fileName = string.Format("{0}_{1:yyyyMMdd-HH}.log", level.ToString(), now);
                    var filePath = Path.Combine(_folderPath, fileName);
                    AsyncFile.Instance.AppendAllText(filePath, message.ToString(), true);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}