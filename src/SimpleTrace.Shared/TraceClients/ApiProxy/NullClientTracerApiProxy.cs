using System;
using System.Threading.Tasks;
using SimpleTrace.Common;

namespace SimpleTrace.TraceClients.ApiProxy
{
    public class NullClientTracerApiProxy : IClientTracerApiProxy
    {
        public Task StartSpan(ClientSpan args)
        {
            return Task.FromResult(0);
        }

        public Task Log(LogArgs args)
        {
            return Task.FromResult(0);
        }

        public Task SetTags(SetTagArgs args)
        {
            return Task.FromResult(0);
        }

        public Task FinishSpan(FinishSpanArgs args)
        {
            return Task.FromResult(0);
        }

        public Task<DateTime> GetDate()
        {
            return Task.FromResult(DateHelper.Instance.GetDateDefault());
        }

        public Task<bool> TryTestApiConnection()
        {
            return Task.FromResult(false);
        }

        public static NullClientTracerApiProxy Instance = new NullClientTracerApiProxy();
        public Task SaveSpans(SaveSpansArgs args)
        {
            return Task.FromResult(0);
        }

        public Task<QueueInfo> GetQueueInfo(GetQueueInfoArgs args)
        {
            return Task.FromResult(new QueueInfo());
        }
    }
}