using System;
using OpenTracing;

namespace SimpleTrace.OpenTrace
{
    public class TracerContext
    {
        public TracerContext(ITracerFactory tracerFactory)
        {
            Factory = tracerFactory;
        }

        public ITracerFactory Factory { get; set; }

        public ITracer Current(string tracerId = null)
        {
            return Factory.GetOrCreateTracer(tracerId);
        }


        #region for simple use
        
        public static ITracer GetCurrent(string tracerId = null)
        {
            var tracer = Resolve().Current(tracerId);
            return tracer;
        }

        #endregion

        #region for di extensions

        private static readonly Lazy<TracerContext> LazyInstance = new Lazy<TracerContext>(() => new TracerContext(new NullTracerFactory()));
        public static Func<TracerContext> Resolve { get; set; } = () => LazyInstance.Value;

        #endregion
    }
}
