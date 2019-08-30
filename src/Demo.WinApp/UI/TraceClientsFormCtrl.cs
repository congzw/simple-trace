using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using SimpleTrace.Common;
using SimpleTrace.TraceClients;
using SimpleTrace.TraceClients.ApiProxy;
using SimpleTrace.TraceClients.Commands;
using SimpleTrace.TraceClients.Repos;
using SimpleTrace.TraceClients.ScheduleTasks;

namespace Demo.WinApp.UI
{
    public class TraceClientsFormCtrl
    {

        public Task<QueueInfo> QueryQueue()
        {
            var apiProxy = ApiProxyContext.Current;
            return apiProxy.GetQueueInfo(new GetQueueInfoArgs());
        }

        public async Task SaveQueue(QueueInfo queueInfo)
        {
            var clientSpanRepository = new ClientSpanRepository(AsyncFile.Instance);

            var commandQueueTask = new CommandQueueTask(new DelayedGroupCacheCommand());

            var clientSpanProcesses = new List<IClientSpanProcess>();
            clientSpanProcesses.Add(new TraceSaveProcess(clientSpanRepository));

            var commandQueue = new CommandQueue();

            //var commands = FilterCommands<StartSpanCommand>(queueInfo);
            var saveCommands = FilterCommands<SaveSpansCommand>(queueInfo).ToList();
            foreach (var command in saveCommands)
            {
                await commandQueue.Enqueue(command);
            }

            await commandQueueTask.Process(clientSpanProcesses, commandQueue,
                DateHelper.Instance.GetDateNow().AddSeconds(-100));
        }

        private IEnumerable<T> FilterCommands<T>(QueueInfo queueInfo) where T : ICommand
        {
            foreach (var queueInfoCommand in queueInfo.Commands)
            {
                if (queueInfoCommand is T theCommand)
                {
                    yield return theCommand;
                }
                else
                {
                    var propName = "CommandType";
                    var tryGetProperty = queueInfoCommand.TryGetProperty(propName, true, out var propValue);

                    if (tryGetProperty)
                    {
                        if (propValue.ToString() == typeof(T).Name)
                        {
                            yield return queueInfoCommand.As<T>();
                        }
                    }
                }
            }


            //var dynamicCommands = queueInfo.Commands.Cast<dynamic>().ToList();
            //foreach (var dynamicCommand in dynamicCommands)
            //{
            //    if (dynamicCommand.CommandType == knownCommands.StartSpan())
            //    {
            //        commands.Add();
            //    }
            //}


            //var commands = queueInfo.Commands.Cast<ICommand>().ToList();
            //return commands;

            //foreach (ICommand queueInfoCommand in queueInfo.Commands)
            //{

            //    var command = queueInfoCommand.As<ICommand>();
            //    await commandQueue.Enqueue(command);
            //}
        }

        public async Task CallTraceApi(CallTraceApiArgs args)
        {
            var apiProxy = ApiProxyContext.Current;
            var saveSpans = new List<SaveClientSpan>();

            var dateHelper = DateHelper.Instance;

            var tracerId = "DemoCallApi-Tracer";

            for (int i = 0; i < args.Count; i++)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(args.IntervalMs));

                var now = dateHelper.GetDateNow();
                var traceId = "Trace_" + now.Ticks;

                var apiSpan1 = CreateSaveClientSpans(tracerId, traceId, null, "SPAN_001", "FooApi");
                var apiSpan2 = CreateSaveClientSpans(tracerId, traceId, "SPAN_001", "SPAN_002", "FooService");
                var apiSpan3 = CreateSaveClientSpans(tracerId, traceId, "SPAN_002", "SPAN_003", "FooRepos", true);

                MockDuration(apiSpan1, now, 0, 500);
                MockDuration(apiSpan2, now, 50, 100);
                MockDuration(apiSpan3, now, 100, 200);

                saveSpans.Add(apiSpan1);
                saveSpans.Add(apiSpan2);
                saveSpans.Add(apiSpan3);
            }

            var saveSpansArgs = SaveSpansArgs.Create(saveSpans.ToArray());
            await apiProxy.SaveSpans(saveSpansArgs);
        }

        private SaveClientSpan CreateSaveClientSpans(string tracerId, string traceId, string parentSpanId, string spanId, string opName, bool withLogs = false)
        {
            var clientSpan = ClientSpan.Create(tracerId, traceId, parentSpanId, spanId, opName);
            var saveClientSpan = new SaveClientSpan();
            MyModelHelper.SetProperties(saveClientSpan, clientSpan);
            if (withLogs)
            {
                saveClientSpan.Logs.Add("foo-log-key", "foo-log-value");
            }
            return saveClientSpan;
        }

        private void MockDuration(SaveClientSpan saveClientSpan, DateTime now, int delayStartMs, int durationMs)
        {
            saveClientSpan.StartUtc = now.AddMilliseconds(delayStartMs);
            saveClientSpan.FinishUtc = saveClientSpan.StartUtc.AddMilliseconds(durationMs);
        }
    }

    public class CallTraceApiArgs
    {
        public int Count { get; set; }
        public int IntervalMs { get; set; }
    }
}
