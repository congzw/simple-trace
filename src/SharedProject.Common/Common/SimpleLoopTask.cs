﻿using System;
using System.Threading;
using System.Threading.Tasks;
// ReSharper disable CheckNamespace

namespace SimpleTrace.Common
{
    public class SimpleLoopTask : IDisposable
    {
        private CancellationTokenSource _cts = null;
        private readonly object _ctsLock = new object();

        public SimpleLoopTask()
        {
            MaxTryFailCount = 3;
            LoopSpan = TimeSpan.FromSeconds(3);
        }

        public TimeSpan LoopSpan { get; set; }
        public Action LoopAction { get; set; }
        public Action AfterExitLoopAction { get; set; }
        public Func<Task> LoopTask { get; set; }
        public Func<Task> AfterExitLoopTask { get; set; }
        public int MaxTryFailCount { get; set; }

        public Task<MessageResult> Start(bool autoStopIfRunning = false)
        {
            lock (_ctsLock)
            {
                if (_cts != null)
                {
                    if (!autoStopIfRunning)
                    {
                        return MessageResult.Create(false, "Task is already running").AsTask();
                    }
                    RunCancelLogic();
                }
                _cts = new CancellationTokenSource();
                var guardTask = Task.Factory.StartNew(RunGuardLoop, _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                return MessageResult.Create(true, "Task is running").AsTask();
            }
        }

        public Task<MessageResult> Stop()
        {
            lock (_ctsLock)
            {
                var result = RunCancelLogic();
                return result.AsTask();
            }
        }

        private void RunGuardLoop()
        {
            int errorCount = 0;
            while (true)
            {
                lock (_ctsLock)
                {
                    if (_cts == null)
                    {
                        LogInfo("NotStarted");
                        break;
                    }
                    if (_cts.IsCancellationRequested)
                    {
                        LogInfo("Cancelled");
                        break;
                    }
                }

                try
                {
                    LoopAction?.Invoke();
                    LoopTask?.Invoke().Wait();
                }
                catch (Exception e)
                {
                    errorCount++;
                    if (errorCount > MaxTryFailCount)
                    {
                        LogInfo(string.Format("fail {0} more then max: {1}, exit", errorCount, MaxTryFailCount));
                        break;
                    }
                    LogEx(e, string.Format("fail time: {0}/{1}, ex:{2}", errorCount, MaxTryFailCount, e.Message));
                }

                try
                {
                    //Thread.Sleep actually makes current thread to sleep
                    //Task.Delay is a logical delay without blocking the current thread. Task.Delay is a timer based wait mechanism.
                    //In async programming model you should always use Task.Delay() if you want something(continuation) happen after some delay.
                    TaskEx.Delay(LoopSpan).Wait();
                }
                catch (TaskCanceledException)
                {
                    //await TaskEx.Delay(LoopSpan); should enter here 
                    LogInfo(string.Format("Task Canceled => TaskCanceledException"));
                }
                catch (AggregateException)
                {
                    //TaskEx.Delay(LoopSpan).Wait(); should enter here 
                    LogInfo(string.Format("Task Canceled => AggregateException"));
                }
                catch (Exception)
                {
                    //should never enter here
                    LogInfo(string.Format("Task Canceled => Exception"));
                }
            }
        }

        private MessageResult RunCancelLogic()
        {
            if (_cts == null)
            {
                return MessageResult.Create(true, "Task is not running");
            }

            LogInfo("Cancelling");
            _cts.Cancel(false);
            _cts.Dispose();
            _cts = null;
            AfterExitLoopAction?.Invoke();
            AfterExitLoopTask?.Invoke().Wait();
            return MessageResult.Create(true, "Task is stopping");
        }

        public void Dispose()
        {
            LogInfo("Disposing");
            RunCancelLogic();
        }

        private void LogInfo(string message)
        {
            var logger = SimpleLogSingleton<SimpleLoopTask>.Instance.Logger;
            logger.LogInfo(message);
        }

        private void LogEx(Exception ex, string message = null)
        {
            var logger = SimpleLogSingleton<SimpleLoopTask>.Instance.Logger;
            logger.LogEx(ex, message);
        }
    }
}