using System;
using OpenTracing;

namespace SimpleTrace.OpenTrace.Jaeger
{
    public class TracerFactory : ITracerFactory
    {
        public string DefaultTracerId { get; set; }
        public ITracer CreateTracer(string tracerId)
        {
            throw new NotImplementedException();
        }

        public ITracer GetOrCreateTracer(string tracerId)
        {
            throw new NotImplementedException();
        }
    }
}
