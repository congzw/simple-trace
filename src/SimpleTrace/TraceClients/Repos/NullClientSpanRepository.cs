using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleTrace.TraceClients.Repos
{
    public class NullClientSpanRepository : IClientSpanRepository
    {
        public Task Clear(LoadArgs args)
        {
            return Task.FromResult(0);
        }

        public Task Add(IList<IClientSpan> spans)
        {
            return Task.FromResult(0);
        }

        public Task<IList<IClientSpan>> Read(LoadArgs args)
        {
            return Task.FromResult(new List<IClientSpan>() as IList<IClientSpan>);
        }
    }
}