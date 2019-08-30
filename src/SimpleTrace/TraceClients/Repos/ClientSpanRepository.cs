using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleTrace.Common;

namespace SimpleTrace.TraceClients.Repos
{
    public class ClientSpanRepository : IClientSpanRepository
    {
        private readonly AsyncFile _asyncFile;

        public ClientSpanRepository(AsyncFile asyncFile)
        {
            _asyncFile = asyncFile;
        }
        public Task Add(IList<ClientSpanEntity> spans)
        {
            if (spans == null || spans.Count == 0)
            {
                return 0.AsTask();
            }
            var content = spans.ToJson(false);
            return _asyncFile.AppendAllText("test.txt", content, true);
        }

        public async Task<IList<ClientSpanEntity>> Read(LoadArgs args)
        {
            //todo filter by args
            var content = await _asyncFile.ReadAllText("test.txt").ConfigureAwait(false);
            var defaultValue = new List<ClientSpanEntity>();
            var result = content.FromJson<IList<ClientSpanEntity>>(defaultValue);
            return result;
        }
    }
}
