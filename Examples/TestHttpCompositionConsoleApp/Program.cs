
namespace TestHttpCompositionConsoleApp
{
    using System;
    using System.Collections.Generic;
    using ConfigurationSources;
    using Serviceable.Objects.Instrumentation.Server;
    using Serviceable.Objects.IO.NamedPipes.Server;
    using Serviceable.Objects.Remote.Composition.Host;
    using Serviceable.Objects.Remote.Composition.Host.Commands;
    using Serviceable.Objects.Remote.Composition.ServiceOrchestrator;
    using Contexts.ConsoleLog;
    using Contexts.Http;
    using Contexts.Queues;
    using Newtonsoft.Json;
    using Serviceable.Objects.Composition.ServiceOrchestrator;
    using Serviceable.Objects.Remote.Composition.Graph;
    using Serviceable.Objects.Remote.Composition.Host.Configuration;
    using Serviceable.Objects.Remote.Composition.ServiceOrchestrator.Configuration;

    class Program
    {
        static void Main(string[] args)
        {
            var serviceOrchestratorGraphTemplate = @"
{
    GraphNodes: [
        { TypeFullName:'" + typeof(ServiceOrchestratorContext).AssemblyQualifiedName + @"', Id:'server-orchestrator-context' },
        { TypeFullName:'" + typeof(InstrumentationServerContext).AssemblyQualifiedName + @"', Id:'instrumentation-context' },
    ],
    GraphVertices: [
    ],
    Registrations: [
    ],
}
";

            var serviceGraphTemplate = @"
{
    GraphNodes: [
        { TypeFullName:'" + typeof(OwinHttpContext).AssemblyQualifiedName + @"', Id:'server-context' },
        { TypeFullName:'" + typeof(QueueContext).AssemblyQualifiedName + @"', Id:'queue-context' },
        { TypeFullName:'" + typeof(ConsoleLogContext).AssemblyQualifiedName + @"', Id:'console-log-context' },
        { TypeFullName:'" + typeof(NamedPipeServerContext).AssemblyQualifiedName + @"', Id:'namedpipes-log-instrumentation-context' },
        { TypeFullName:'" + typeof(InstrumentationServerContext).AssemblyQualifiedName + @"', Id:'instrumentation-context' },
    ],
    GraphVertices: [
        { FromId:'server-context', ToId:'queue-context', },
        { FromId:'queue-context', ToId:'console-log-context',  },
        { FromId:'namedpipes-log-instrumentation-context', ToId:'console-log-context',  },
    ],
    Registrations: [
        { Type:'" + typeof(ServiceContainerConfigurationSource).AssemblyQualifiedName + @"', WithDefaultInterface:true },
    ],
}
";
            /*
             * Principles:
             * 
             * Testable
             * Composable
             * Configurable
             * Instrumentable
             * Scalable
             * Updatable TODO
             * 
             */

            if (args.Length > 0)
            {
                Console.WriteLine("Starting service...");
                new ApplicationHost(args).Execute(new RunAndBlock());
                return;
            }

            Console.WriteLine("Starting orchestrator...");

            var serviceOrchestratorConfiguration = new ServiceOrchestratorConfiguration
            {
                EntryAssemblyFullPath = "C:\\sources\\serviceable-objects\\Examples\\TestHttpCompositionConsoleApp\\bin\\Debug\\netcoreapp1.0\\TestHttpCompositionConsoleApp.dll",
                OrchestratorName = "orchestrator-X",
                GraphTemplatesDictionary = new Dictionary<string, string>
                {
                    ["service-X"] = serviceGraphTemplate,
                },
                InBindingsPerService = new Dictionary<string, List<InBinding>>
                {
                    ["service-X"] = new List<InBinding>
                    {
                        new InBinding
                        {
                            ContextTypeName = typeof(OwinHttpContext).AssemblyQualifiedName,
                            ScaleSetBindings = new List<Binding>
                            {
                                new Binding { Host = "localhost", Port = "5000" },
                                new Binding { Host = "localhost", Port = "5001" },
                            }
                        }
                    }
                }
            };

            new ApplicationHost(JsonConvert.SerializeObject(new ApplicationHostDataConfiguration
            {
                ServiceOrchestratorConfiguration = serviceOrchestratorConfiguration,
                OrchestratorOverrideTemplate = JsonConvert.DeserializeObject<GraphTemplate>(serviceOrchestratorGraphTemplate),
            }))
            .Execute(new RunAndBlock());
        }
    }
}