using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using SimpleTrace.TraceClients.Commands;

namespace SimpleTrace.TraceClients.ScheduleTasks
{
    public class CommandQueueTaskLoop : IDisposable
    {
        public CommandQueueTaskLoop()
        {
            this.ReportAdd();
            Log = SimpleLogSingleton<CommandQueueTaskLoop>.Instance.Logger;
        }

        public SimpleLoopTask LoopTask { get; set; }

        public void Init(TimeSpan? loopSpan,
            CommandQueueTask commandQueueTask,
            Func<CommandQueue> getCommandQueue,
            Func<IEnumerable<ICommandLogistic>> getCommandLogistics,
            Func<IEnumerable<IClientSpanProcess>> getClientSpanProcesses,
            Func<DateTime> getNow)
        {
            if (LoopTask != null)
            {
                LogInfo("LoopTask already started!!!");
                return;
            }

            if (commandQueueTask == null)
            {
                throw new ArgumentNullException(nameof(commandQueueTask));
            }

            if (getCommandQueue == null)
            {
                throw new ArgumentNullException(nameof(getCommandQueue));
            }

            if (getCommandLogistics == null)
            {
                throw new ArgumentNullException(nameof(getCommandLogistics));
            }

            if (getClientSpanProcesses == null)
            {
                throw new ArgumentNullException(nameof(getClientSpanProcesses));
            }

            if (getNow == null)
            {
                throw new ArgumentNullException(nameof(getNow));
            }
            
            LoopTask = new SimpleLoopTask();
            if (loopSpan != null)
            {
                LoopTask.LoopSpan = loopSpan.Value;
            }

            LoopTask.LoopTask = () =>
            {
                LogInfo(string.Format(">>> CommandQueueTaskLoop is looping at {0:yyyy-MM-dd HH:mm:ss:fff} in thread {1}", DateTime.Now, Thread.CurrentThread.ManagedThreadId));
                var commandQueue = getCommandQueue();
                var commandLogistics = getCommandLogistics();
                var processes = getClientSpanProcesses();
                var now = getNow();
                return commandQueueTask.ProcessQueue(commandQueue, commandLogistics.ToList(), processes.ToList(), now);
            };

            LoopTask.AfterExitLoopTask = () =>
            {
                LogInfo(string.Format(">>> CommandQueueTaskLoop is stopping at {0:yyyy-MM-dd HH:mm:ss:fff} in thread {1}", DateTime.Now, Thread.CurrentThread.ManagedThreadId));
                return Task.FromResult(0);
            };
        }

        protected bool Started { get; set; }

        public void Start()
        {
            if (LoopTask == null)
            {
                LogInfo("LoopTask is not init!!!");
                return;
            }

            if (Started)
            {
                LogInfo("LoopTask is already started!!!");
                return;
            }

            LoopTask.Start();
            Started = true;
        }
        
        private void LogInfo(string message)
        {
            Log.LogInfo(message);
        }

        public ISimpleLog Log { get; set; }

        public void Dispose()
        {
            LoopTask?.Dispose();
            this.ReportDelete();
        }

        public static CommandQueueTaskLoop Instance = new CommandQueueTaskLoop();
    }
}
