using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using OpenTracing;

namespace SimpleTrace.OpenTrace
{
    public class TracerCache
    {
        private TracerCache(ITracerFactory tracerFactory)
        {
            Factory = tracerFactory;
            CachedTracers = new ConcurrentDictionary<string, ITracer>(StringComparer.OrdinalIgnoreCase);
        }

        public ITracerFactory Factory { get; set; }
        public ITracer GetOrCreate(string tracerId)
        {
            var tryFixTraceId = TryFixTracerId(tracerId);
            if (!CachedTracers.ContainsKey(tryFixTraceId))
            {
                CachedTracers[tryFixTraceId] = Factory.CreateTracer(tryFixTraceId);
            }
            return CachedTracers[tryFixTraceId];
        }
        public IDictionary<string, ITracer> CachedTracers { get; set; }

        //helpers
        private string TryFixTracerId(string tracerId)
        {
            return tracerId ?? Factory.DefaultTracerId;
        }
        public static TracerCache Create(ITracerFactory tracerFactory)
        {
            return new TracerCache(tracerFactory);
        }
    }
}
