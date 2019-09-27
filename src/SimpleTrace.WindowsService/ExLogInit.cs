using System;
using System.IO;
using SimpleTrace.Common;

namespace SimpleTrace.Ws
{
    public class ExLogInit
    {
        private static string _folderPath = null;
        public static ISimpleLogFactory SetupLogEx()
        {
            var simpleLogFactory = SimpleLogFactory.Resolve();
            var initLog = simpleLogFactory.CreateLogFor<ExLogInit>();
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

        private static void LogExToFile(LogMessageArgs args)
        {
            var level = args.Level;
            var message = args.Message;
            if (level >= SimpleLogLevel.Error && level < SimpleLogLevel.None)
            {
                var now = DateHelper.Instance.GetDateNow();
                var fileName = string.Format("{0}_{1}.log", level.ToString(), now.ToString("yyyyMMdd-HH"));
                var filePath = Path.Combine(_folderPath, fileName);
                AsyncFile.Instance.AppendAllText(filePath, message.ToString(), true);
            }
        }
    }
}
