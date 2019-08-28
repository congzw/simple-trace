using System.Threading.Tasks;

namespace SimpleTrace.TraceClients.Api
{
    public interface IClientTracerApi
    {
        Task StartSpan(ClientSpan args);
        Task Log(LogArgs args);
        Task SetTags(SetTagArgs args);
        Task FinishSpan(FinishSpanArgs args);

        /// <summary>
        /// save multi spans with only one call
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        Task SaveSpans(SaveSpansArgs args);
    }
}