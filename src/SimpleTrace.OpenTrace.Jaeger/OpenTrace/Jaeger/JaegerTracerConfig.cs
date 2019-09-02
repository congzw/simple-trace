namespace SimpleTrace.OpenTrace.Jaeger
{
    public class JaegerTracerConfig
    {
        public JaegerTracerConfig()
        {
            TraceEndPoint = "http://localhost:14268/api/traces";
            DefaultTracerId = "Default-Tracer";
        }

        public string DefaultTracerId { get; set; }
        public string TraceEndPoint { get; set; }
    }
}