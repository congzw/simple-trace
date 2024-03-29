﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SimpleTrace.Common;
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
        public CommandQueueTask CommandQueueTask { get; set; }
        public CommandQueue CommandQueue { get; set; }
        public IList<ICommandLogistic> CommandLogistics { get; set; }
        public IList<IClientSpanProcess> ClientSpanProcesses { get; set; }


        public void Init(TimeSpan? loopSpan,
            CommandQueueTask commandQueueTask,
            CommandQueue commandQueue,
            IEnumerable<ICommandLogistic> commandLogistics,
            IEnumerable<IClientSpanProcess> clientSpanProcesses,
            Func<DateTime> getNow)
        {
            if (LoopTask != null)
            {
                LogInfo("LoopTask already started!!!");
                return;
            }

            CommandQueueTask = commandQueueTask ?? throw new ArgumentNullException(nameof(commandQueueTask));

            CommandQueue = commandQueue ?? throw new ArgumentNullException(nameof(commandQueue));

            CommandLogistics = commandLogistics == null ? throw new ArgumentNullException(nameof(commandQueue)) : commandLogistics.ToList();

            ClientSpanProcesses = clientSpanProcesses == null ? throw new ArgumentNullException(nameof(clientSpanProcesses)) : clientSpanProcesses.ToList();

            if (getNow == null)
            {
                throw new ArgumentNullException(nameof(getNow));
            }
            
            LoopTask = new SimpleLoopTask();
            if (loopSpan != null)
            {
                LoopTask.LoopSpan = loopSpan.Value;
            }

            LogInfo(string.Format(">>> LoopSpanSeconds: {0}", LoopTask.LoopSpan.TotalSeconds));

            LoopTask.LoopTask = () =>
            {
                LogInfo(string.Format(">>> looping at {0:yyyy-MM-dd HH:mm:ss} in thread {1}", DateTime.Now, Thread.CurrentThread.ManagedThreadId));
                var now = getNow();
                return commandQueueTask.ProcessQueue(commandQueue, CommandLogistics, ClientSpanProcesses, now);
            };

            LoopTask.AfterExitLoopTask = () =>
            {
                LogInfo(string.Format(">>> stopping at {0:yyyy-MM-dd HH:mm:ss} in thread {1}", DateTime.Now, Thread.CurrentThread.ManagedThreadId));
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
