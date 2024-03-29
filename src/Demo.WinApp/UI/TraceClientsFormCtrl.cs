﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimpleTrace.Common;
using SimpleTrace.OpenTrace.Jaeger;
using SimpleTrace.TraceClients;
using SimpleTrace.TraceClients.ApiProxy;
using SimpleTrace.TraceClients.Commands;
using SimpleTrace.TraceClients.Repos;
using SimpleTrace.TraceClients.ScheduleTasks;

namespace Demo.WinApp.UI
{
    public class TraceClientsFormCtrl
    {
        public IList<ClientSpan> CreateSaveClientSpans(CallTraceApiArgs args)
        {
            var saveSpans = new List<ClientSpan>();

            var dateHelper = DateHelper.Instance;
            var tracerId = "DemoTracer-" + DateHelper.Instance.GetNowAsFormat("yyyyMMddHHmm");

            var theTime = dateHelper.GetDateNow();
            for (int i = 0; i < args.Count; i++)
            {
                //Task.Delay(TimeSpan.FromMilliseconds(args.IntervalMs)).Wait();
                var now = theTime.AddMilliseconds(args.IntervalMs * i);
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

            return saveSpans;
        }

        public async Task CallTraceApi(CallTraceApiArgs args)
        {
            var saveSpans = CreateSaveClientSpans(args);
            var saveSpansArgs = SaveSpansArgs.Create(saveSpans.ToArray());

            var apiProxy = ApiProxyContext.Current;
            await apiProxy.SaveSpans(saveSpansArgs);
        }

        public Task<QueueInfo> QueryQueue()
        {
            var apiProxy = ApiProxyContext.Current;
            var getQueueInfoArgs = new GetQueueInfoArgs();
            getQueueInfoArgs.SetIncludes(GetQueueInfoArgs.Commands, GetQueueInfoArgs.CommandSums);
            return apiProxy.GetQueueInfo(getQueueInfoArgs);
        }

        public Task ProcessQueueInfo(QueueInfo queueInfo)
        {
            var clientSpanEntities = GetClientSpanEntities(queueInfo);
            var jaegerTraceSender = new JaegerTraceSender();
            return jaegerTraceSender.Send(clientSpanEntities);
        }

        public async Task ProcessQueue(QueueInfo queueInfo)
        {
            var simpleIoc = SimpleIoc.Instance;
            var commandQueueTask = simpleIoc.Resolve<CommandQueueTask>();
            var knownCommands = simpleIoc.Resolve<KnownCommands>();
            var commandQueue = simpleIoc.Resolve<CommandQueue>();
            var clientSpanProcesses = simpleIoc.Resolve<IList<IClientSpanProcess>>();

            //var dequeueCommands = await commandQueueTask.DequeueCommands(commandQueue);
            //var clientSpanEntities = commandQueueTask.GetEntities(knownCommands.CommandLogistics, dequeueCommands, DateHelper.Instance.GetDateNow().AddSeconds(-100));
            //await commandQueueTask.ProcessEntities(clientSpanProcesses, clientSpanEntities);

            await commandQueueTask.ProcessQueue(commandQueue, 
                knownCommands.CommandLogistics, 
                clientSpanProcesses, 
                DateHelper.Instance.GetDateNow().AddSeconds(-100));
        }

        public Task Save(IList<ClientSpan> clientSpanEntities)
        {
            var clientSpanRepository = new ClientSpanRepository(AsyncFile.Instance);
            var clientSpans = clientSpanEntities.Cast<IClientSpan>().ToList();
            return clientSpanRepository.Add(clientSpans);
        }

        public Task<IList<IClientSpan>> Load(LoadArgs args)
        {
            var clientSpanRepository = new ClientSpanRepository(AsyncFile.Instance);
            return clientSpanRepository.Read(args);
        }

        public Task Delete(LoadArgs args)
        {
            var clientSpanRepository = new ClientSpanRepository(AsyncFile.Instance);
            return clientSpanRepository.Clear(args);
        }
        
        private IList<IClientSpan> GetClientSpanEntities(QueueInfo queueInfo)
        {
            var simpleIoc = SimpleIoc.Instance;
            var commandQueueTask = simpleIoc.Resolve<CommandQueueTask>();
            var knownCommands = simpleIoc.Resolve<KnownCommands>();
            var commands = queueInfo.Commands.FromJTokenOrObject<Command>().ToList();
            var clientSpanEntities = commandQueueTask.GetEntities(knownCommands.CommandLogistics, commands, DateHelper.Instance.GetDateNow().AddSeconds(-100));
            return clientSpanEntities;
        }

        private ClientSpan CreateSaveClientSpans(string tracerId, string traceId, string parentSpanId, string spanId, string opName, bool withLogs = false)
        {
            var clientSpan = ClientSpan.Create(tracerId, traceId, parentSpanId, spanId, opName);
            var saveClientSpan = new ClientSpan();
            MyModelHelper.SetProperties(saveClientSpan, clientSpan);
            if (withLogs)
            {
                saveClientSpan.Logs.Add("foo-log-key", LogItem.Create("foo-log-key", "foo-log-value", null));
                saveClientSpan.Tags.Add("foo-tag-key", "foo-tag-value");
            }
            return saveClientSpan;
        }

        private void MockDuration(IClientSpan saveClientSpan, DateTime now, int delayStartMs, int durationMs)
        {
            saveClientSpan.StartUtc = now.AddMilliseconds(delayStartMs);

            foreach (var logItem in saveClientSpan.Logs)
            {
                logItem.Value.CreateAt = now.AddMilliseconds(delayStartMs);
            }
            saveClientSpan.FinishUtc = saveClientSpan.StartUtc.AddMilliseconds(durationMs);
        }
    }

    public class CallTraceApiArgs
    {
        public int Count { get; set; }
        public int IntervalMs { get; set; }
    }
}
