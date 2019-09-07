using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            Runners = new List<SimpleProcessRunner>();
            LoopTask = new SimpleLoopTask();
            Init(LoopTask);
        }

        public IList<SimpleProcessRunner> Runners { get; set; }

        private void Init(SimpleLoopTask loopTask)
        {
            var processInfos = GetProcessInfos("SimpleTrace.ini");
            foreach (var processInfo in processInfos)
            {
                //var jaegerInfo = SimpleProcessInfo.Create("jaeger-all-in-one", "jaeger-all-in-one.exe", "--collector.zipkin.http-port=9411");
                var process = SimpleProcess.GetOrCreate(processInfo);
                var processRunner = new SimpleProcessRunner(process);
                LogInfo(string.Format("Init ProcessRunner: [{0}]", processInfo.ProcessName));
                Runners.Add(processRunner);
            }
            
            loopTask.LoopSpan = TimeSpan.FromSeconds(15);
            loopTask.LoopAction = () =>
            {
                LogInfo("looping check trace service at " + DateHelper.Instance.GetDateNow().ToString("s"));
                foreach (var runner in Runners)
                {
                    runner.TryStart();
                }
            };

            loopTask.AfterExitLoopAction = () =>
            {
                LogInfo("stopping check trace service at " + DateHelper.Instance.GetDateNow().ToString("s"));
                foreach (var runner in Runners)
                {
                    runner.TryStop();
                }
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

        private IList<SimpleProcessInfo> GetProcessInfos(string iniFileName)
        {
            var processInfos = new List<SimpleProcessInfo>();

            var simpleIniFile = SimpleIni.ResolveFile();
            var fullPath = Path.GetFullPath(AppDomain.CurrentDomain.Combine(iniFileName));
            if (!File.Exists(fullPath))
            {
                LogInfo("IniFile Not Exist: " + fullPath);
                return processInfos;
            }
            
            var traceIniItems = simpleIniFile.TryLoadIniFileItems(fullPath);
            if (traceIniItems == null)
            {
                LogInfo(fullPath + " has no items value ");
                return processInfos;
            }

            LogInfo(string.Format("-----{0}----", iniFileName));
            foreach (var item in traceIniItems)
            {
                LogInfo(string.Format("{0}: {1}", item.Key, item.Value));
            }

            var processesValue = simpleIniFile.GetValue(traceIniItems, "Daemon", "Processes");
            if (processesValue == null)
            {
                LogInfo("[Daemon]Processes is null!");
                return processInfos;
            }

            var processSections = processesValue.ToString().Trim().Split(new[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();

            foreach (var processSection in processSections)
            {
                //[TraceApi]
                //ProcessName="SimpleTrace.Api"
                //ExePath="TraceApi\SimpleTrace.Api.exe"
                //ExeArgs=

                var processNameValue = simpleIniFile.GetValue(traceIniItems, processSection, "ProcessName");
                var exePathValue = simpleIniFile.GetValue(traceIniItems, processSection, "ExePath");
                var exeArgsValue = simpleIniFile.GetValue(traceIniItems, processSection, "ExeArgs");

                var processName = processNameValue == null ? string.Empty : processNameValue.ToString();
                var exePath = exePathValue == null ? string.Empty : exePathValue.ToString();
                var exeArgs = exeArgsValue == null ? string.Empty : exeArgsValue.ToString();

                if (string.IsNullOrWhiteSpace(processName))
                {
                    LogInfo(string.Format("SimpleTrace.ini => [{0}]ProcessName is null!", processSection));
                    continue;
                }

                if (string.IsNullOrWhiteSpace(exePath))
                {
                    LogInfo(string.Format("SimpleTrace.ini => [{0}]ExePath is null!", processSection));
                    continue;
                }

                var fullExePath = Path.GetFullPath(AppDomain.CurrentDomain.Combine(exePath));
                var processInfo = SimpleProcessInfo.Create(processName, fullExePath, exeArgs);
                LogInfo(string.Format("----{0}----", processSection));
                LogInfo(MyModelHelper.MakeIniString(processInfo));
                processInfos.Add(processInfo);
            }
            return processInfos;
        }
    }
}
