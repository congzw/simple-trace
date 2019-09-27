using System;
using System.Threading;
using SimpleTrace.Common;

namespace Demo.WinApp.UI
{
    public class SimpleLoopTaskCtrl
    {
        public SimpleLoopTaskCtrl()
        {
            Log = SimpleLogSingleton<SimpleLoopTaskCtrl>.Instance.Logger;
        }
        
        public SimpleLoopTask LoopTask { get; set; }
        
        private void Init()
        {
            LoopTask = new SimpleLoopTask();
            LoopTask.LoopSpan = TimeSpan.FromSeconds(3);
            LoopTask.LoopAction = () =>
            {
                Log.LogInfo(string.Format(">>> demo long running task is running at {0:yyyy-MM-dd HH:mm:ss:fff} in thread {1}", DateTime.Now, Thread.CurrentThread.ManagedThreadId));
            };
            LoopTask.AfterExitLoopAction = () =>
            {
                Log.LogInfo(string.Format(">>> demo long running task is stopping at {0:yyyy-MM-dd HH:mm:ss:fff} in thread {1}", DateTime.Now, Thread.CurrentThread.ManagedThreadId));
            };
        }

        public void Start(TimeSpan? loopSpan)
        {
            if (LoopTask != null)
            {
                Log.LogInfo("LoopTask already started!!!");
                return;
            }

            Init();

            if (loopSpan != null)
            {
                LoopTask.LoopSpan = loopSpan.Value;
            }
            LoopTask.Start();
        }

        public void Stop()
        {
            if (LoopTask == null)
            {
                Log.LogInfo("LoopTask already stopped!!!");
                return;
            }

            LoopTask.Stop();
            LoopTask.Dispose();
            LoopTask = null;
        }

        public ISimpleLog Log { get; set; }
    }
}
