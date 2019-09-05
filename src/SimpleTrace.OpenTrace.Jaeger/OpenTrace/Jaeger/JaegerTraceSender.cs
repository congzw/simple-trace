using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using SimpleTrace.TraceClients;
using SimpleTrace.TraceClients.Sends;

namespace SimpleTrace.OpenTrace.Jaeger
{
    public class JaegerTraceSender : ITraceSender
    {
        public Task Send(IList<IClientSpan> entities)
        {
            //1 group and convert trace span trees
            //2 send spans by relations
            var sameTraceSpanGroups = entities.OrderBy(x => x.StartUtc)
                .GroupBy(x => new { x.TracerId, x.TraceId });

            var tracerContext = TracerContext.Resolve();

            foreach (var sameTraceSpanGroup in sameTraceSpanGroups)
            {
                var logger = SimpleLogSingleton<JaegerTracerFactory>.Instance.Logger;
                logger.LogInfo(string.Format("Send ClientSpans => {0}: {1}", sameTraceSpanGroup.Key, sameTraceSpanGroup.Count()));

                //var groupKey = sameTraceSpanGroup.Key;
                var sameTraceSpans = sameTraceSpanGroup.ToList();
                var myTreeConvert = MyTreeConvert.Instance;
                var myTrees = myTreeConvert
                    .MakeTrees(sameTraceSpans, span => span.ToLocateCurrentKey(), span => span.ToLocateParentKey())
                    .OrderBy(x => x.Value.StartUtc).ToList();

                foreach (var myTree in myTrees)
                {
                    SendSpanTree(tracerContext, myTree);
                }
            }

            return Task.FromResult(0);
        }

        private void SendSpanTree(TracerContext tracerContext, MyTree<IClientSpan> spanTree)
        {
            var clientSpan = spanTree.Value;
            var tracer = tracerContext.Current(clientSpan.TracerId);

            using (var scope = tracer.BuildSpan(clientSpan.OpName)
                .WithStartTimestamp(clientSpan.StartUtc)
                .StartActive(false))
            {

                foreach (var bag in clientSpan.Bags)
                {
                    if (bag.Value == null)
                    {
                        scope.Span.SetTag(bag.Key, null);
                        continue;
                    }
                    scope.Span.SetTag(bag.Key, bag.Value);
                }

                var logGroups = clientSpan.Logs.GroupBy(x => x.Value.CreateAt).ToList();
                foreach (var logGroup in logGroups)
                {
                    var createAt = logGroup.Key;
                    var logInfos = logGroup.Select(x => new KeyValuePair<string, object>(x.Key, x.Value.Value)).ToList();
                    scope.Span.Log(createAt, logInfos);
                }


                foreach (var tag in clientSpan.Tags)
                {
                    if (tag.Value == null)
                    {
                        scope.Span.SetTag(tag.Key, null);
                        continue;
                    }

                    if (tag.Value is bool boolValue)
                    {
                        scope.Span.SetTag(tag.Key, boolValue);
                    }
                    else if (tag.Value is int intValue)
                    {
                        scope.Span.SetTag(tag.Key, intValue);
                    }
                    else if (tag.Value is double doubleValue)
                    {
                        scope.Span.SetTag(tag.Key, doubleValue);
                    }
                    scope.Span.SetTag(tag.Key, tag.Value.ToString());
                }

                //cascade process children
                foreach (var childTree in spanTree.Children)
                {
                    SendSpanTree(tracerContext, childTree);
                }

                if (clientSpan.FinishUtc != null)
                {
                    //throw ex?
                    scope.Span.Finish(clientSpan.FinishUtc.Value);
                }
            }
        }
    }
}
