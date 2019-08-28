using OpenTracing;

namespace SimpleTrace.OpenTrace
{
    public interface ITracerFactory
    {
        string DefaultTracerId { get; set; }
        ITracer CreateTracer(string tracerId);
        ITracer GetOrCreateTracer(string tracerId);
    }
}
