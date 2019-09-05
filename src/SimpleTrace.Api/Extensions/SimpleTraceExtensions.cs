using System;
using System.Linq;
using System.Reflection;
using Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleTrace.OpenTrace;
using SimpleTrace.OpenTrace.Jaeger;
using SimpleTrace.TraceClients.Api;
using SimpleTrace.TraceClients.Commands;
using SimpleTrace.TraceClients.Repos;
using SimpleTrace.TraceClients.ScheduleTasks;
using SimpleTrace.TraceClients.Sends;

namespace SimpleTrace.Api.Extensions
{
    public static class SimpleTraceExtensions
    {
        public static void AddSimpleTrace(this IServiceCollection services, IConfiguration configuration)
        {
            AddCommon(services);
            AddTraceClients(services, configuration);
        }

        public static void ConfigSimpleTrace(this IApplicationBuilder app, IHostingEnvironment env, IConfiguration configuration)
        {
            ConfigLoopTask(app);
        }

        private static void ConfigLoopTask(IApplicationBuilder app)
        {
            var queueTask = app.ApplicationServices.GetService<CommandQueueTask>();
            var knownCommands = app.ApplicationServices.GetService<KnownCommands>();
            var commandLogistics = app.ApplicationServices.GetServices<ICommandLogistic>().ToList();
            foreach (var commandLogistic in commandLogistics)
            {
                knownCommands.Register(commandLogistic);
            }
            
            var queueTaskLoop = app.ApplicationServices.GetService<CommandQueueTaskLoop>();
            queueTaskLoop.Init(
                TimeSpan.FromSeconds(3),
                queueTask,
                app.ApplicationServices.GetService<CommandQueue>,
                app.ApplicationServices.GetServices<ICommandLogistic>,
                app.ApplicationServices.GetServices<IClientSpanProcess>,
                DateHelper.Instance.GetDateNow);

            queueTaskLoop.Start();
        }
        private static void AddCommon(IServiceCollection services)
        {
            services.AddSingleton(SimpleJson.Resolve() as SimpleJson);
            services.AddSingleton(SimpleJson.Resolve());
            services.AddSingleton(SimpleJson.ResolveSimpleJsonFile());

            services.AddSingleton(SimpleLogFactory.Resolve() as SimpleLogFactory);
            services.AddSingleton(SimpleLogFactory.Resolve());

            services.AddSingleton(WebApiHelper.Resolve() as WebApiHelper);
            services.AddSingleton(WebApiHelper.Resolve());

            //also can use like this: 
            //services.AddSingleton<WebApiHelper>();
            //services.AddSingleton<IWebApiHelper>(sp => sp.GetService<WebApiHelper>());
            //WebApiHelper.Resolve = () => app.ApplicationServices.GetService<IWebApiHelper>(); //in Config process
        }
        private static void AddTraceClients(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IClientTracerApi, ClientTracerApi>();
            services.AddSingleton<CommandQueue>();
            services.AddSingleton<CommandQueueTask>();
            services.AddSingleton(CommandQueueTaskLoop.Instance);

            services.AddSingleton(KnownCommands.Instance);
            var assemblyToScan = Assembly.GetAssembly(typeof(ICommandLogistic)); //..or assembly you need
            services.AddSingletonFromAssembly(assemblyToScan, typeof(ICommandLogistic));

            services.AddSingleton(AsyncFile.Instance);
            services.AddSingleton<IClientSpanRepository, ClientSpanRepository>();
            services.AddSingleton<IClientSpanProcess, TraceSaveProcess>();

            //by config

            var tracerSection = configuration.GetSection("Tracer");
            var traceEndPoint = tracerSection["TraceEndPoint"];
            var defaultTracerId = tracerSection["DefaultTracerId"];
            var traceEnabledValue = tracerSection["Enabled"] ?? "false";
            var traceArchiveFolder = tracerSection["TraceArchiveFolder"];
            var flushIntervalSecondInt = int.Parse(tracerSection["FlushIntervalSecond"]);
            var flushIntervalSecond = TimeSpan.FromSeconds(flushIntervalSecondInt);
            bool.TryParse(traceEnabledValue, out var traceEnabled);
            if (traceEnabled)
            {
                var jaegerTracerConfig = new JaegerTracerConfig();
                jaegerTracerConfig.DefaultTracerId = defaultTracerId;
                jaegerTracerConfig.TraceEndPoint = traceEndPoint;
                services.AddSingleton(jaegerTracerConfig);

                var tracerFactory = new JaegerTracerFactory(jaegerTracerConfig);
                services.AddSingleton<ITracerFactory>(tracerFactory);

                var tracerContext = TracerContext.Resolve();
                tracerContext.Factory = tracerFactory;
                services.AddSingleton<TracerContext>(tracerContext);

                var tracer = tracerContext.Current();

                services.AddSingleton<ITraceSender, JaegerTraceSender>();
                services.AddSingleton<IClientSpanProcess, TraceSendProcess>();


                using (var scope = tracer.BuildSpan("Setup").StartActive(true))
                {
                    scope.Span.SetTag("ITracerFactory", "JaegerFactory");
                    scope.Span.Log("DefaultTracerId: " + defaultTracerId);
                    scope.Span.Log("TraceEndPoint: " + traceEndPoint);
                    scope.Span.Log("FlushIntervalSecond: " + flushIntervalSecondInt);
                    scope.Span.Log("TraceArchiveFolder: " + traceArchiveFolder);
                }
            }
        }

        public static void AddSingletonFromAssembly(this IServiceCollection services, Assembly assembly, Type interfaceType)
        {
            var allServices = assembly.GetTypes()
                .Where(p => p.GetTypeInfo().IsClass && !p.GetTypeInfo().IsAbstract && interfaceType.IsAssignableFrom(p))
                .ToList();
            foreach (var service in allServices)
            {
                services.AddSingleton(interfaceType, service);
            }
        }
    }
}