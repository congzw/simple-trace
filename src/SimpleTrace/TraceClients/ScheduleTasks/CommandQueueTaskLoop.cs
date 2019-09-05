using System;
using System.Collections.Generic;
using System.Threading;
using Common;

namespace SimpleTrace.TraceClients.ScheduleTasks
{
    public class CommandQueueTaskLoop : IDisposable
    {
        public CommandQueueTaskLoop()
        {
            this.ReportAdd();
            //Log = SimpleLogSingleton<CommandQueueLoopTask>.Instance.Logger;
            CacheMessages = new List<string>();
        }

        public SimpleLoopTask LoopTask { get; set; }

        private void Init()
        {
            LoopTask = new SimpleLoopTask();
            LoopTask.LoopSpan = TimeSpan.FromSeconds(3);
            LoopTask.LoopAction = () =>
            {
                LogInfo(string.Format(">>> demo long running task is running at {0:yyyy-MM-dd HH:mm:ss:fff} in thread {1}", DateTime.Now, Thread.CurrentThread.ManagedThreadId));
            };
            LoopTask.AfterExitLoopAction = () =>
            {
                LogInfo(string.Format(">>> demo long running task is stopping at {0:yyyy-MM-dd HH:mm:ss:fff} in thread {1}", DateTime.Now, Thread.CurrentThread.ManagedThreadId));
            };
        }

        public void Start(TimeSpan? loopSpan)
        {
            if (LoopTask != null)
            {
                LogInfo("LoopTask already started!!!");
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
                LogInfo("LoopTask already stopped!!!");
                return;
            }

            LoopTask.Stop();
            LoopTask.Dispose();
            LoopTask = null;
        }

        public IList<string> CacheMessages { get; set; }

        private void LogInfo(string message)
        {
            CacheMessages.Add(message);
            //Log.LogInfo(message);
        }

        //public ISimpleLog Log { get; set; }
        public void Dispose()
        {
            this.ReportDelete();
            LoopTask?.Dispose();
        }
    }
}
