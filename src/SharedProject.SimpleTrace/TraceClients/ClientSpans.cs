using System;
using System.Collections.Generic;
using SimpleTrace.Common;

namespace SimpleTrace.TraceClients
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
        public static MessageResult ValidateSave(this IClientSpan clientSpan, bool validateFinish)
        {
            var result = MessageResult.FailResult(string.Empty);
            if (clientSpan == null)
            {
                result.Message = "Span不能空";
                return result;
            }

            result.Data = clientSpan;

            var isBadLocateArgs = clientSpan.IsBadLocateArgs(ClientSpanLocateMode.ForCurrent);
            if (isBadLocateArgs)
            {
                result.Message = "Span的关键KEY不能空";
                return result;
            }

            //if (string.IsNullOrWhiteSpace(clientSpan.OpName))
            //{
            //    return false;
            //}

            if (clientSpan.StartUtc == default(DateTime))
            {
                result.Message = "StartUtc必须赋值";
                return result;
            }

            if (validateFinish && clientSpan.FinishUtc == null)
            {
                result.Message = "FinishUtc必须赋值";
                return result;
            }

            if (clientSpan.FinishUtc == default(DateTime))
            {
                result.Message = "FinishUtc必须赋值";
                return result;
            }

            result.Success = true;
            return result;
        }

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
}
