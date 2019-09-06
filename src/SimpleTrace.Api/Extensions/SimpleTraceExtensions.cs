using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleTrace.OpenTrace;
using SimpleTrace.OpenTrace.Jaeger;
using SimpleTrace.TraceClients;
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
            AddTraceClients(services, configuration);
        }

        public static void ConfigSimpleTrace(this IApplicationBuilder app, IHostingEnvironment env, IConfiguration configuration)
        {
            var queueTaskLoop = app.ApplicationServices.GetService<CommandQueueTaskLoop>();
            queueTaskLoop.Start();
        }

        private static void AddTraceClients(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IClientTracerApi, ClientTracerApi>();
            services.AddSingleton<CommandQueue>();
            services.AddSingleton<CommandQueueTask>();

            //SimpleTrace.dll
            var assemblyToScan = Assembly.GetAssembly(typeof(ICommandLogistic));

            //All IClientSpanProcess
            services.AddSingletonFromAssembly(assemblyToScan, typeof(IClientSpanProcess));
            //All ICommandLogistic and KnownCommands
            services.AddSingletonFromAssembly(assemblyToScan, typeof(ICommandLogistic));
            services.AddSingleton(sp =>
            {
                var knownCommands = KnownCommands.Instance;

                var commandLogistics = sp.GetServices<ICommandLogistic>().ToList();
                foreach (var commandLogistic in commandLogistics)
                {
                    knownCommands.Register(commandLogistic);
                }

                return knownCommands;
            });

            //TraceConfig
            var traceConfig = TryLoadTraceConfig(configuration);
            services.AddSingleton(traceConfig);

            //CommandQueueTaskLoop
            services.AddSingleton(sp =>
            {
                var commandQueueTaskLoop = new CommandQueueTaskLoop();
                
                var queueTask = sp.GetService<CommandQueueTask>();
                var knownCommands = sp.GetService<KnownCommands>();
                var commandLogistics = sp.GetServices<ICommandLogistic>().ToList();
                foreach (var commandLogistic in commandLogistics)
                {
                    knownCommands.Register(commandLogistic);
                }

                commandQueueTaskLoop.Init(
                    TimeSpan.FromSeconds(traceConfig.FlushIntervalSecond),
                    queueTask,
                    sp.GetService<CommandQueue>(),
                    sp.GetServices<ICommandLogistic>(),
                    sp.GetServices<IClientSpanProcess>(),
                    DateHelper.Instance.GetDateNow);

                return commandQueueTaskLoop;
            });

            //IClientSpanRepository
            if (traceConfig.TraceSaveProcessEnabled)
            {
                services.AddSingleton<IClientSpanRepository, ClientSpanRepository>();
            }
            else
            {
                services.AddSingleton<IClientSpanRepository, NullClientSpanRepository>();
            }

            //TracerContext
            var tracerContext = TracerContext.Resolve();
            services.AddSingleton<TracerContext>(tracerContext);

            if (traceConfig.TraceSendProcessEnabled)
            {
                //JaegerTracerConfig
                var jaegerTracerConfig = new JaegerTracerConfig();
                jaegerTracerConfig.DefaultTracerId = traceConfig.DefaultTracerId;
                jaegerTracerConfig.TraceEndPoint = traceConfig.TraceEndPoint;
                services.AddSingleton(jaegerTracerConfig);

                //ITracerFactory and TracerContext
                var tracerFactory = new JaegerTracerFactory(jaegerTracerConfig);
                services.AddSingleton<ITracerFactory>(tracerFactory);
                tracerContext.Factory = tracerFactory;

                //ITraceSender
                services.AddSingleton<ITraceSender, JaegerTraceSender>();

                //log TraceConfig
                var tracer = tracerContext.Current();
                using (var scope = tracer.BuildSpan("LogObject").StartActive(true))
                {
                    scope.Span.SetTag("Name", "TraceConfig");
                    var dictionary = MyModelHelper.GetKeyValueDictionary(traceConfig);
                    foreach (var kv in dictionary)
                    {
                        scope.Span.Log(kv.Key + " : " + kv.Value);
                    }
                }
            }
            else
            {
                services.AddSingleton<ITracerFactory, NullTracerFactory>();
                services.AddSingleton<ITraceSender, NullTraceSender>();
            }
        }
        private static TraceConfig TryLoadTraceConfig(IConfiguration configuration)
        {
            var isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            var traceConfig = isLinux ? TraceConfig.CreateDefaultForLinux() : TraceConfig.CreateDefaultForWindows();


            var tracerSection = configuration.GetSection("Tracer");
            if (tracerSection != null)
            {
                ////not use this to process nullable props
                //var config = tracerSection.Get<TraceConfig>();

                var traceSaveProcessEnabled = tracerSection.GetValue<bool?>("TraceSaveProcessEnabled", null);
                if (traceSaveProcessEnabled != null)
                {
                    traceConfig.TraceSaveProcessEnabled = traceSaveProcessEnabled.Value;
                }

                var traceSendProcessEnabled = tracerSection.GetValue<bool?>("TraceSendProcessEnabled", null);
                if (traceSendProcessEnabled != null)
                {
                    traceConfig.TraceSendProcessEnabled = traceSendProcessEnabled.Value;
                }

                var traceEndPoint = tracerSection.GetValue<string>("TraceEndPoint", null);
                if (!string.IsNullOrWhiteSpace(traceEndPoint))
                {
                    traceConfig.TraceEndPoint = traceEndPoint.Trim();
                }

                var defaultTracerId = tracerSection.GetValue<string>("DefaultTracerId", null);
                if (!string.IsNullOrWhiteSpace(defaultTracerId))
                {
                    traceConfig.DefaultTracerId = defaultTracerId.Trim();
                }

                var flushIntervalSecond = tracerSection.GetValue<int?>("FlushIntervalSecond", null);
                if (flushIntervalSecond != null)
                {
                    traceConfig.FlushIntervalSecond = flushIntervalSecond.Value;
                }

                var traceArchiveFolder = tracerSection.GetValue<string>("TraceArchiveFolder", null);
                if (!string.IsNullOrWhiteSpace(traceArchiveFolder))
                {
                    traceConfig.TraceArchiveFolder = traceArchiveFolder.Trim();
                }
            }

            return traceConfig;
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