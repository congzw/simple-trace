using System;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using Common;

namespace SimpleTrace.Ws
{
    public partial class SimpleTraceService : ServiceBase
    {
        public SimpleTraceService()
        {
            InitializeComponent();
            LoopTask = new SimpleLoopTask();
            Init(LoopTask);
        }

        private void Init(SimpleLoopTask loopTask)
        {
            var jaegerInfo = SimpleProcessInfo.Create("jaeger-all-in-one", "jaeger-all-in-one.exe", "--collector.zipkin.http-port=9411");
            var jaegerProcess = SimpleProcess.GetOrCreate(jaegerInfo);
            var jaegerRunner = new SimpleProcessRunner(jaegerProcess);

            var traceApiInfo = SimpleProcessInfo.Create("Zonekey.EzTrace.Api", "Zonekey.EzTrace.Api.exe", null);
            var traceApiProcess = SimpleProcess.GetOrCreate(traceApiInfo);
            var traceApiRunner = new SimpleProcessRunner(traceApiProcess);

            //update by config 
            var simpleIniFile = SimpleIni.ResolveFile();

            var fullPath = Path.GetFullPath(AppDomain.CurrentDomain.Combine("EzTrace.ini"));
            var traceIniItems = simpleIniFile.TryLoadIniFileItems(fullPath);
            LogInfo("EzTrace.ini => " + fullPath);

            if (traceIniItems != null)
            {
                simpleIniFile.SetProperties(traceIniItems, jaegerInfo, "Jaeger");
                simpleIniFile.SetProperties(traceIniItems, traceApiInfo, "TraceApi");
            }
            // ProcessName=jaeger-all-in-one;ExePath=Jaeger\jaeger-all-in-one.exe;ExeArgs=--collector.zipkin.http-port=9411 
            LogInfo("----JaegerInfo----");
            LogInfo(MyModelHelper.MakeIniString(jaegerInfo));
            LogInfo("----TraceApiInfo----");
            LogInfo(MyModelHelper.MakeIniString(traceApiInfo));

            loopTask.LoopSpan = TimeSpan.FromSeconds(15);
            loopTask.LoopAction = () =>
            {
                LogInfo("looping check trace service");
                jaegerRunner.TryStart();
                traceApiRunner.TryStart();

            };

            loopTask.AfterExitLoopAction = () =>
            {
                LogInfo("exiting check trace service");
                jaegerRunner.TryStop();
                traceApiRunner.TryStop();
            };
        }

        public SimpleLoopTask LoopTask { get; set; }

        protected override void OnStart(string[] args)
        {
            LogInfo(string.Format("OnStart begin {0:yyyy-MM-dd HH:mm:ss:fff} in thread {1}", DateTime.Now, Thread.CurrentThread.ManagedThreadId));
            LoopTask.Start();
            LogInfo(string.Format("OnStart end {0:yyyy-MM-dd HH:mm:ss:fff} in thread {1}", DateTime.Now, Thread.CurrentThread.ManagedThreadId));
        }

        protected override void OnStop()
        {
            LogInfo(string.Format("OnStop begin {0:yyyy-MM-dd HH:mm:ss:fff} in thread {1}", DateTime.Now, Thread.CurrentThread.ManagedThreadId));
            LoopTask.Stop();
            LogInfo(string.Format("OnStop end {0:yyyy-MM-dd HH:mm:ss:fff} in thread {1}", DateTime.Now, Thread.CurrentThread.ManagedThreadId));
        }

        private ISimpleLog _simpleLog = null;
        private void LogInfo(string info)
        {
            if (_simpleLog == null)
            {
                _simpleLog = SimpleLogSingleton<SimpleTraceService>.Instance.Logger;
            }
            _simpleLog.LogInfo(info);
        }
    }
}
