using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Jaeger.Senders;
using OpenTracing;

namespace SimpleTrace.OpenTrace.Jaeger
{
    public class JaegerTracerFactory : ITracerFactory
    {
        public JaegerTracerFactory(JaegerTracerConfig config)
        {
            Config = config;
            DefaultTracerId = config.DefaultTracerId;
            CachedTracers = TracerCache.Create(this);
        }

        public JaegerTracerConfig Config { get; set; }

        public TracerCache CachedTracers { get; set; }

        public string DefaultTracerId { get; set; }

        public ITracer CreateTracer(string tracerId)
        {
            return CreateTracer(Config.TraceEndPoint, tracerId);
        }

        public ITracer GetOrCreateTracer(string tracerId)
        {
            return CachedTracers.GetOrCreate(tracerId);
        }

        //helpers
        private Tracer CreateTracer(string endPoint, string serviceName)
        {
            var traceBuilder = new Tracer.Builder(serviceName)
                .WithSampler(new ConstSampler(true));
                //.WithLoggerFactory(_loggerFactory);

            //var loggerFactory = traceBuilder.LoggerFactory;
            var metrics = traceBuilder.Metrics;

            //14268:OK
            var sender = new HttpSender(endPoint);

            var reporter = new RemoteReporter.Builder()
                //.WithLoggerFactory(_loggerFactory)
                .WithMetrics(metrics)
                .WithSender(sender)
                .Build();

            var tracer = traceBuilder
                .WithReporter(reporter)
                .Build();

            return tracer;
        }
    }
}
