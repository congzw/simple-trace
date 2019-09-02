using System;
using System.Threading.Tasks;

namespace SimpleTrace.TraceClients.Api
{
    public interface ITestApi
    {
        Task<DateTime> GetDate();
    }
}