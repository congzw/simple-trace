using System;
using System.ServiceProcess;
using Common;

namespace SimpleTraceWs
{
    static class Program
    {
        private static ISimpleLogFactory _simpleLogFactory = null;

        static void Main()
        {
            _simpleLogFactory = ExLogInit.SetupLogEx();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            var servicesToRun = new ServiceBase[]
            {
                new SimpleTraceService()
            };
            ServiceBase.Run(servicesToRun);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                var simpleLog = _simpleLogFactory.GetOrCreateLogFor(typeof(Program));
                simpleLog.LogEx(ex);
            }
        }
    }
}
