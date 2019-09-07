using System;
using System.Collections.Generic;
using System.Linq;
using Common;

namespace SimpleTrace.TraceClients
{
    public class StartArgs : IClientSpanLocate
    {
        public StartArgs()
        {
            Bags = DictionaryHelper.CreateDictionary<string>();
        }
        
        public string TracerId { get; set; }
        public string TraceId { get; set; }
        public string SpanId { get; set; }
        public string ParentSpanId { get; set; }
        public string OpName { get; set; }
        public IDictionary<string, string> Bags { get; set; }
        
        public static StartArgs Create(string tracerId, string traceId, string parentSpanId, string spanId, string opName, IDictionary<string, string> bags = null)
        {
            var args = new StartArgs();

            //todo validate
            args.TracerId = tracerId;
            args.TraceId = traceId;
            args.SpanId = spanId;
            args.ParentSpanId = parentSpanId;
            args.OpName = opName;
            
            if (bags != null)
            {
                args.Bags = bags;
            }
            return args;
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

    //接口JSON序列化有问题
    public class SaveSpansArgs : IBatchClientSpanLocate<ClientSpan>
    {
        public SaveSpansArgs()
        {
            Items = new List<ClientSpan>();
        }
        public IList<ClientSpan> Items { get; set; }

        public static SaveSpansArgs Create(params IClientSpan[] clientSpans)
        {
            var spans = new List<ClientSpan>();
            foreach (var someImpl in clientSpans)
            {
                if (someImpl == null)
                {
                    continue;
                }

                if (someImpl is ClientSpan)
                {
                    spans.Add((ClientSpan)someImpl);
                }
                else
                {
                    var clientSpan = new ClientSpan();
                    MyModelHelper.SetProperties(clientSpan, someImpl);
                    spans.Add(clientSpan);
                }
            }
            
            var saveSpansArgs = new SaveSpansArgs { Items = spans };
            return saveSpansArgs;
        }

        public static MessageResult Validate(SaveSpansArgs args)
        {
            var result = MessageResult.FailResult(string.Empty);
            if (args == null)
            {
                result.Message = "参数不能为空";
                return result;
            }

            if (args.Items == null)
            {
                result.Message = "Items不能为空";
                return result;
            }

            if (args.Items.Count == 0)
            {
                result.Message = "Items数量不能为0";
                return result;
            }


            foreach (var clientSpan in args.Items)
            {
                var validateSave = clientSpan.ValidateSave(true);
                if (!validateSave.Success)
                {
                    return validateSave;
                }
            }

            result.Success = true;
            return result;
        }
    }

    public class ClientSpan : IClientSpan
    {
        public ClientSpan()
        {
            Bags = DictionaryHelper.CreateDictionary<string>();
            Tags = DictionaryHelper.CreateDictionary<object>();
            Logs = DictionaryHelper.CreateDictionary<LogItem>();
        }

        public string TracerId { get; set; }
        public string TraceId { get; set; }
        public string SpanId { get; set; }
        public string ParentSpanId { get; set; }

        public string OpName { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime? FinishUtc { get; set; }

        public IDictionary<string, string> Bags { get; set; }
        public IDictionary<string, object> Tags { get; set; }
        public IDictionary<string, LogItem> Logs { get; set; }

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
    
    public class QueueInfo
    {
        public QueueInfo()
        {
            Commands = new List<object>();
            CommandSums = new List<CommandSum>();
        }

        public int TotalCount { get; set; }
        public IList<object> Commands { get; set; }
        public IList<CommandSum> CommandSums { get; set; }
    }

    public class CommandSum
    {
        public string CommandType { get; set; }
        public int CommandCount { get; set; }
    }

    public class GetQueueInfoArgs
    {
        public static string CommandSums = "CommandSums";
        public static string Commands = "Commands";

        private readonly IncludeProperties _includeProperties = IncludeProperties.Create();

        public string Includes { get; set; }

        public bool Include(string name)
        {
            _includeProperties.Properties = Includes;
            return _includeProperties.HasProperty(name);
        }

        public GetQueueInfoArgs SetInclude(string name)
        {
            _includeProperties.SetProperty(name, true);
            Includes = _includeProperties.Properties;
            return this;
        }
        
        public GetQueueInfoArgs SetIncludes(params string[] names)
        {
            _includeProperties.SetProperties( true, names);
            Includes = _includeProperties.Properties;
            return this;
        }
    }
}
