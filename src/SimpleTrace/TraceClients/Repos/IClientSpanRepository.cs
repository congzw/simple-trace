using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleTrace.TraceClients.Repos
{
    public interface IClientSpanRepository
    {
        Task Add(IList<ClientSpanEntity> spans);
        Task<IList<ClientSpanEntity>> Read(LoadArgs args);
    }

    public class LoadArgs
    {
        public DateTime Begin { get; set; }
        public DateTime? End { get; set; }
    }
}
