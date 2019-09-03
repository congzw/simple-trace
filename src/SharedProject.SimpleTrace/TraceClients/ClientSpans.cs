using System;
using System.Collections.Generic;
using Common;

namespace SimpleTrace.TraceClients.V2
{
    public interface IClientSpan : IClientSpanLocate
    {
        string OpName { get; set; }
        DateTime StartUtc { get; set; }
        DateTime? FinishUtc { get; set; }
        IDictionary<string, string> Bags { get; set; }
        IDictionary<string, object> Tags { get; set; }
        IDictionary<string, LogItem> Logs { get; set; }
    }

    public static class ClientSpanExtensions
    {
        public static bool CanFinish(this IClientSpan clientSpan)
        {
            return clientSpan.FinishUtc.HasValue;
        }

        public static T SetBags<T>(this T clientSpan, IEnumerable<KeyValuePair<string, string>> bags) where T : IClientSpan
        {
            if (clientSpan.Bags == null)
            {
                return clientSpan;
            }

            foreach (var item in bags)
            {
                clientSpan.Bags[item.Key] = item.Value;
            }
            return clientSpan;
        }

        public static T SetTags<T>(this T clientSpan, IEnumerable<KeyValuePair<string, object>> tags) where T : IClientSpan
        {
            if (clientSpan.Tags == null)
            {
                return clientSpan;
            }

            foreach (var item in tags)
            {
                clientSpan.Tags[item.Key] = item.Value;
            }
            return clientSpan;
        }

        public static T SetLogs<T>(this T clientSpan, IEnumerable<KeyValuePair<string, object>> logs, DateTime? createAt = null) where T : IClientSpan
        {
            if (clientSpan.Logs == null)
            {
                return clientSpan;
            }

            var items = LogItem.Create(logs, createAt);
            foreach (var item in items)
            {
                clientSpan.Logs[item.Key] = item;
            }

            return clientSpan;
        }
    }
    
    public class LogItem
    {
        public LogItem()
        {
            CreateAt = DateHelper.Instance.GetDateNow();
        }

        public DateTime CreateAt { get; set; }
        public string Key { get; set; }
        public object Value { get; set; }

        public static IList<LogItem> Create(IEnumerable<KeyValuePair<string, object>> infos, DateTime? createAt)
        {
            var items = new List<LogItem>();
            foreach (var info in infos)
            {
                var keyValueInfo = Create(info.Key, info.Value, createAt);
                items.Add(keyValueInfo);
            }
            return items;
        }
        public static LogItem Create(string key, object value, DateTime? createAt)
        {
            var item = new LogItem();
            item.Key = key;
            item.Value = value;
            if (createAt != null)
            {
                item.CreateAt = createAt.Value;
            }
            return item;
        }
    }

    public class ClientSpan : IClientSpanLocate
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

    }
}
