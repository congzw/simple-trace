using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common
{
    public class AsyncQueue<T>
    {
        private readonly AsyncSemaphore _lock = new AsyncSemaphore(1);
        public AsyncQueue()
        {
            Items = new ConcurrentQueue<T>();
        }

        public ConcurrentQueue<T> Items { get; set; }

        public async Task Enqueue(T item)
        {
            await _lock.WaitAsync().ConfigureAwait(false);
            try
            {
                Items.Enqueue(item);
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<T> TryDequeue()
        {
            await _lock.WaitAsync().ConfigureAwait(false);
            try
            {
                Items.TryDequeue(out T result);
                return result;
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<IList<T>> TryDequeueAll()
        {
            await _lock.WaitAsync().ConfigureAwait(false);
            try
            {
                IList<T> results = new List<T>();
                while (!Items.IsEmpty)
                {
                    var tryDequeue = Items.TryDequeue(out T result);
                    if (tryDequeue)
                    {
                        results.Add(result);
                    }
                }

                return results;
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}