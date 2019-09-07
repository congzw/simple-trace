using System;
using System.ServiceProcess;
using System.Threading;
using Common;

namespace SimpleTraceWs
{
    public partial class SimpleTraceService : ServiceBase
    {
        public SimpleTraceService()
        {
            InitializeComponent();
        }


        protected override void OnStart(string[] args)
        {
            LogInfo(string.Format("OnStart begin {0:yyyy-MM-dd HH:mm:ss:fff} in thread {1}", DateTime.Now, Thread.CurrentThread.ManagedThreadId));
            //LoopTask.Start();
            LogInfo(string.Format("OnStart end {0:yyyy-MM-dd HH:mm:ss:fff} in thread {1}", DateTime.Now, Thread.CurrentThread.ManagedThreadId));
        }

        protected override void OnStop()
        {
            LogInfo(string.Format("OnStop begin {0:yyyy-MM-dd HH:mm:ss:fff} in thread {1}", DateTime.Now, Thread.CurrentThread.ManagedThreadId));
            //LoopTask.Stop();
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
