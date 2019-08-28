using System.Threading.Tasks;
using SimpleTrace.TraceClients.Api;

namespace SimpleTrace.TraceClients.ApiProxy
{
    public interface IClientTracerApiProxy : IClientTracerApi, ITestApi
    {
        Task<bool> TryTestApiConnection();
    }
}