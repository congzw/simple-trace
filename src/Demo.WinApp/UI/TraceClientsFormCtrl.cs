using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleTrace.Common;
using SimpleTrace.TraceClients;
using SimpleTrace.TraceClients.ApiProxy;

namespace Demo.WinApp.UI
{
    public class TraceClientsFormCtrl
    {
        public async Task CallTraceApi(CallTraceApiArgs args)
        {
            var apiProxy = ApiProxyContext.Current;
            var saveSpans = new List<SaveClientSpan>();

            var dateHelper = DateHelper.Instance;

            var tracerId = "DemoCallApi-Tracer";

            for (int i = 0; i < args.Count; i++)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(args.Interval));

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

        private void MockDuration(SaveClientSpan saveClientSpan, DateTime now,  int delayStartMs, int durationMs)
        {
            saveClientSpan.StartUtc = now.AddMilliseconds(delayStartMs);
            saveClientSpan.FinishUtc = saveClientSpan.StartUtc.AddMilliseconds(durationMs);
        }
    }

    public class CallTraceApiArgs
    {
        public int Count { get; set; }
        public int Interval { get; set; }
    }
}
