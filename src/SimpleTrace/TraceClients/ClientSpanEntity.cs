using System;
using System.Collections.Generic;

namespace SimpleTrace.TraceClients
{
    public class ClientSpanEntity : IClientSpanLocate
    {
        public ClientSpanEntity()
        {
            Bags = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Logs = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            Tags = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public string TracerId { get; set; }
        public string TraceId { get; set; }
        public string SpanId { get; set; }
        public string ParentSpanId { get; set; }
        public string OpName { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime FinishUtc { get; set; }

        public IDictionary<string, string> Bags { get; set; }
        public IDictionary<string, object> Logs { get; set; }
        public IDictionary<string, object> Tags { get; set; }

        public void SetBags(IEnumerable<KeyValuePair<string, string>> bags)
        {
            if (bags == null)
            {
                return;
            }

            foreach (var item in bags)
            {
                this.Bags[item.Key] = item.Value;
            }
        }

        public void SetLogs(IEnumerable<KeyValuePair<string, object>> logs)
        {
            if (logs == null)
            {
                return;
            }

            foreach (var item in logs)
            {
                this.Logs[item.Key] = item.Value;
            }
        }

        public void SetTags(IEnumerable<KeyValuePair<string, object>> tags)
        {
            if (tags == null)
            {
                return;
            }

            foreach (var item in tags)
            {
                this.Tags[item.Key] = item.Value;
            }
        }
    }
}