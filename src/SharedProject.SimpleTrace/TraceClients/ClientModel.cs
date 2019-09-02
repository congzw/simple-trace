using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace SimpleTrace.TraceClients
{
    public class ClientSpan : IClientSpanLocate
    {
        public ClientSpan()
        {
            Bags = DictionaryHelper.CreateDictionary<string>();
        }

        public string TracerId { get; set; }
        public string TraceId { get; set; }
        public string SpanId { get; set; }
        public string ParentSpanId { get; set; }
        public string OpName { get; set; }

        //for extensions
        public IDictionary<string, string> Bags { get; set; }
        
        public static ClientSpan Create(string tracerId, string traceId, string parentSpanId, string spanId, string opName, IDictionary<string, string> bags = null)
        {
            var theSpan = new ClientSpan();
            Set(theSpan, tracerId, traceId, parentSpanId, spanId, opName, bags);
            return theSpan;
        }

        public static ClientSpan Set(ClientSpan clientSpan, string tracerId, string traceId, string parentSpanId, string spanId, string opName, IDictionary<string, string> bags = null)
        {
            if (bags != null)
            {
                clientSpan.Bags = bags;
            }

            //todo validate
            clientSpan.TracerId = tracerId;
            clientSpan.TraceId = traceId;
            clientSpan.SpanId = spanId;
            clientSpan.ParentSpanId = parentSpanId;
            clientSpan.OpName = opName;

            return clientSpan;
        }
    }

    public class LogArgs : IClientSpanLocate
    {
        public LogArgs()
        {
            Logs = DictionaryHelper.CreateDictionary<object>();
        }

        public string TracerId { get; set; }
        public string TraceId { get; set; }
        public string SpanId { get; set; }
        public string ParentSpanId { get; set; }

        public IDictionary<string, object> Logs { get; set; }

        public LogArgs WithLog(string key, object value)
        {
            //todo validate
            Logs[key] = value;
            return this;
        }
    }

    public class SetTagArgs : IClientSpanLocate
    {
        public SetTagArgs()
        {
            Tags = DictionaryHelper.CreateDictionary<object>();
        }

        public string TracerId { get; set; }
        public string TraceId { get; set; }
        public string SpanId { get; set; }
        public string ParentSpanId { get; set; }

        public IDictionary<string, object> Tags { get; set; }

        public SetTagArgs WithTag(string key, object value)
        {
            //todo validate
            Tags[key] = value;
            return this;
        }
    }

    public class FinishSpanArgs : IClientSpanLocate
    {
        public string TracerId { get; set; }
        public string TraceId { get; set; }
        public string SpanId { get; set; }
        public string ParentSpanId { get; set; }
    }

    public class SaveSpansArgs : IBatchClientSpanLocate<SaveClientSpan>
    {
        public SaveSpansArgs()
        {
            Items = new List<SaveClientSpan>();
        }
        public IList<SaveClientSpan> Items { get; set; }

        public static SaveSpansArgs Create(params SaveClientSpan[] saveClientSpans)
        {
            var saveSpansArgs = new SaveSpansArgs { Items = saveClientSpans.ToList() };
            return saveSpansArgs;
        }
    }

    public class SaveClientSpan : ClientSpan
    {
        public SaveClientSpan()
        {
            Logs = DictionaryHelper.CreateDictionary<object>();
            Tags = DictionaryHelper.CreateDictionary<object>();
            StartUtc = DateHelper.Instance.GetDateNow();
        }

        public IDictionary<string, object> Logs { get; set; }
        public IDictionary<string, object> Tags { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime FinishUtc { get; set; }
    }

    public class QueueInfo
    {
        public QueueInfo()
        {
            Commands = new List<object>();
        }

        public int TotalCount { get; set; }
        public IList<object> Commands { get; set; }
    }
    
    public class GetQueueInfoArgs
    {
        //todo
    }

    internal class DictionaryHelper
    {
        public static IDictionary<string, T> CreateDictionary<T>(bool ignoreCase = true, bool concurrent = false)
        {
            if (concurrent)
            {
                return ignoreCase ? new ConcurrentDictionary<string, T>(StringComparer.OrdinalIgnoreCase) : new ConcurrentDictionary<string, T>();
            }

            return ignoreCase ? new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase) : new Dictionary<string, T>();
        }
    }
}
