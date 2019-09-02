using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimpleTrace.Common;
using SimpleTrace.TraceClients;
using SimpleTrace.TraceClients.Sends;

namespace SimpleTrace.OpenTrace.Jaeger
{
    public class JaegerTraceSender : ITraceSender
    {
        public Task Send(IList<ClientSpanEntity> entities)
        {
            //1 group and convert trace span trees
            //2 send spans by relations
            var sameTraceSpanGroups = entities.OrderBy(x => x.StartUtc)
                .GroupBy(x => new { x.TracerId, x.TraceId });

            var tracerContext = TracerContext.Resolve();

            foreach (var sameTraceSpanGroup in sameTraceSpanGroups)
            {
                Console.WriteLine("{0}: {1}", sameTraceSpanGroup.Key, sameTraceSpanGroup.Count());
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

        private void SendSpanTree(TracerContext tracerContext, MyTree<ClientSpanEntity> spanTree)
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

                scope.Span.Log(clientSpan.Logs);

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
                scope.Span.Finish(clientSpan.FinishUtc);
            }
        }
    }
}
