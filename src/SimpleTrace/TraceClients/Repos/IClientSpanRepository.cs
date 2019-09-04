using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleTrace.TraceClients.Repos
{
    public interface IClientSpanRepository
    {
        Task Add(IList<IClientSpan> spans);
        Task<IList<IClientSpan>> Read(LoadArgs args);
        Task Clear(LoadArgs args);
    }

    public class LoadArgs
    {
        public DateTime? Begin { get; set; }
        public DateTime? End { get; set; }
    }
}
