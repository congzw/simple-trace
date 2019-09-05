using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable CheckNamespace

namespace SimpleTrace.Api.Controllers
{
    [Route("api/loop")]
    public class DemoLoopTaskController : ControllerBase
    {
        //use get just for easy test 
        [Route("Set")]
        [HttpGet]
        public Task<string> Set(bool? enabled, int? spanSec)
        {
            var demoLoopTask = DemoLoopTask.Instance;
            if (enabled.HasValue)
            {
                if (enabled.Value)
                {
                    if (spanSec.HasValue)
                    {
                        demoLoopTask.Start(TimeSpan.FromSeconds(spanSec.Value));
                    }
                    else
                    {
                        demoLoopTask.Start(null);
                    }
                }
                else
                {
                    demoLoopTask.Stop();
                }
            }
            return Task.FromResult(string.Format("Set => Enabled:{0}, Span:{1}", enabled , spanSec));
        }

        [Route("GetInfos")]
        [HttpGet]
        public Task<IList<string>> GetInfos()
        {
            var demoLoopTask = DemoLoopTask.Instance;
            return Task.FromResult(demoLoopTask.CacheMessages);
        }
    }

    public class DemoLoopTask : IDisposable
    {
        public DemoLoopTask()
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

        public static DemoLoopTask Instance = new DemoLoopTask();
    }
}
