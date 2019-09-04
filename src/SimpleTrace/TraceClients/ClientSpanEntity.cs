//using System;
//using System.Collections.Generic;
//using Common;

//namespace SimpleTrace.TraceClients
//{
//    public class ClientSpanEntity : IClientSpanLocate
//    {
//        public ClientSpanEntity()
//        {
//            Bags = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
//            Tags = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
//            Logs = new Dictionary<string, KeyValueInfo>(StringComparer.OrdinalIgnoreCase);
//        }

//        public string TracerId { get; set; }
//        public string TraceId { get; set; }
//        public string SpanId { get; set; }
//        public string ParentSpanId { get; set; }
//        public string OpName { get; set; }
//        public DateTime StartUtc { get; set; }
//        public DateTime FinishUtc { get; set; }

//        public IDictionary<string, string> Bags { get; set; }
//        public IDictionary<string, object> Tags { get; set; }
//        public IDictionary<string, KeyValueInfo> Logs { get; set; }

//        public void SetBags(IEnumerable<KeyValuePair<string, string>> bags)
//        {
//            if (bags == null)
//            {
//                return;
//            }

//            foreach (var item in bags)
//            {
//                this.Bags[item.Key] = item.Value;
//            }
//        }

//        public void SetLogs(IEnumerable<KeyValuePair<string, object>> logs, DateTime? createAt = null)
//        {
//            if (logs == null)
//            {
//                return;
//            }

//            var keyValueInfos = KeyValueInfo.Create(logs, createAt);
//            foreach (var keyValueInfo in keyValueInfos)
//            {
//                this.Logs[keyValueInfo.KeyValuePair.Key] = keyValueInfo;
//            }
//        }

//        public void SetTags(IEnumerable<KeyValuePair<string, object>> tags)
//        {
//            if (tags == null)
//            {
//                return;
//            }

//            foreach (var tag in tags)
//            {
//                this.Tags[tag.Key] = tag.Value;
//            }
//        }
//    }

//    public class KeyValueInfo
//    {
//        public KeyValueInfo()
//        {
//            CreateAt = DateHelper.Instance.GetDateNow();
//        }

//        public DateTime CreateAt { get; set; }

//        public KeyValuePair<string, object> KeyValuePair { get; set; }

//        public static IList<KeyValueInfo> Create(IEnumerable<KeyValuePair<string, object>> infos, DateTime? createAt)
//        {
//            var keyValueInfos = new List<KeyValueInfo>();
//            foreach (var info in infos)
//            {
//                var keyValueInfo = Create(info, createAt);
//                keyValueInfos.Add(keyValueInfo);
//            }
//            return keyValueInfos;
//        }
//        public static KeyValueInfo Create(KeyValuePair<string, object> info, DateTime? createAt)
//        {
//            var keyValueInfo = new KeyValueInfo();
//            if (createAt != null)
//            {
//                keyValueInfo.CreateAt = createAt.Value;
//            }

//            keyValueInfo.KeyValuePair = new KeyValuePair<string, object>(info.Key, info.Value);
//            return keyValueInfo;
//        }
//    }
//}