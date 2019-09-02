using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using OpenTracing;
using OpenTracing.Mock;

namespace SimpleTrace.OpenTrace
{
    public class NullTracerFactory : ITracerFactory
    {
        public NullTracerFactory()
        {
            DefaultTracerId = NullTracer;
            CachedTracers = TracerCache.Create(this);
        }

        public TracerCache CachedTracers { get; set; }

        public string DefaultTracerId { get; set; }
        public ITracer CreateTracer(string tracerId)
        {
            var tracer = new MockTracer();
            return tracer;
        }

        public ITracer GetOrCreateTracer(string tracerId)
        {
            return CachedTracers.GetOrCreate(tracerId);
        }

        //helpers
        public static string NullTracer = "Null-Tracer";
    }
}
