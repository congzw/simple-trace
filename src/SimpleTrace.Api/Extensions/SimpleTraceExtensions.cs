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
            ConfigLoopTask(app);
        }

        private static void ConfigLoopTask(IApplicationBuilder app)
        {
            var traceConfig = app.ApplicationServices.GetService<TraceConfig>();

            var queueTask = app.ApplicationServices.GetService<CommandQueueTask>();
            var knownCommands = app.ApplicationServices.GetService<KnownCommands>();
            var commandLogistics = app.ApplicationServices.GetServices<ICommandLogistic>().ToList();
            foreach (var commandLogistic in commandLogistics)
            {
                knownCommands.Register(commandLogistic);
            }
            
            var queueTaskLoop = app.ApplicationServices.GetService<CommandQueueTaskLoop>();
            queueTaskLoop.Init(
                TimeSpan.FromSeconds(traceConfig.FlushIntervalSecond),
                queueTask,
                app.ApplicationServices.GetService<CommandQueue>,
                app.ApplicationServices.GetServices<ICommandLogistic>,
                app.ApplicationServices.GetServices<IClientSpanProcess>,
                DateHelper.Instance.GetDateNow);

            queueTaskLoop.Start();
        }
        private static void AddTraceClients(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IClientTracerApi, ClientTracerApi>();
            services.AddSingleton<CommandQueue>();
            services.AddSingleton<CommandQueueTask>();
            services.AddSingleton(CommandQueueTaskLoop.Instance);

            services.AddSingleton(KnownCommands.Instance);

            //SimpleTrace.dll
            var assemblyToScan = Assembly.GetAssembly(typeof(ICommandLogistic)); 
            services.AddSingletonFromAssembly(assemblyToScan, typeof(ICommandLogistic));
            services.AddSingletonFromAssembly(assemblyToScan, typeof(IClientSpanProcess));
            
            var traceConfig = TryLoadTraceConfig(configuration);
            services.AddSingleton(traceConfig);

            //setup trace by config
            if (traceConfig.TraceSaveProcessEnabled)
            {
                services.AddSingleton<IClientSpanRepository, ClientSpanRepository>();
            }
            else
            {
                services.AddSingleton<IClientSpanRepository, NullClientSpanRepository>();
            }

            var tracerContext = TracerContext.Resolve();
            services.AddSingleton<TracerContext>(tracerContext);

            if (traceConfig.TraceSendProcessEnabled)
            {
                var jaegerTracerConfig = new JaegerTracerConfig();
                jaegerTracerConfig.DefaultTracerId = traceConfig.DefaultTracerId;
                jaegerTracerConfig.TraceEndPoint = traceConfig.TraceEndPoint;
                services.AddSingleton(jaegerTracerConfig);

                var tracerFactory = new JaegerTracerFactory(jaegerTracerConfig);
                services.AddSingleton<ITracerFactory>(tracerFactory);
                services.AddSingleton<ITraceSender, JaegerTraceSender>();
                tracerContext.Factory = tracerFactory;
                
                var tracer = tracerContext.Current();
                using (var scope = tracer.BuildSpan("ShowInfo").StartActive(true))
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