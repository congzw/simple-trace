using System.Threading.Tasks;

namespace SimpleTrace.TraceClients.Api
{
    public interface IClientTracerApi
    {
        Task StartSpan(ClientSpan args);
        Task Log(LogArgs args);
        Task SetTags(SetTagArgs args);
        Task FinishSpan(FinishSpanArgs args);

        //save multi spans with only one call
        Task SaveSpans(SaveSpansArgs args);

        //for peek current queue infos
        Task<QueueInfo> GetQueueInfo(GetQueueInfoArgs args);
    }
}