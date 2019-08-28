using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleTrace.TraceClients.Repos
{
    public class NullClientSpanRepository : IClientSpanRepository
    {
        public Task Add(IList<ClientSpanEntity> spans)
        {
            return Task.FromResult(0);
        }

        public Task<IList<ClientSpanEntity>> Read(LoadArgs args)
        {
            return Task.FromResult(new List<ClientSpanEntity>() as IList<ClientSpanEntity>);
        }
    }
}