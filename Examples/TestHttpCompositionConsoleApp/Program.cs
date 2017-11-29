
namespace TestHttpCompositionConsoleApp
{
    using ConfigurationSources;
    using Serviceable.Objects.Composition.Graph;
    using Serviceable.Objects.Composition.ServiceOrchestrator;
    using Serviceable.Objects.Dependencies;
    using Serviceable.Objects.Instrumentation.Server;
    using Serviceable.Objects.IO.NamedPipes.Server;
    using Serviceable.Objects.Remote.Composition.Graph;
    using Serviceable.Objects.Remote.Composition.Host;
    using Serviceable.Objects.Remote.Composition.Host.Commands;
    using Serviceable.Objects.Remote.Composition.Service;
    using Serviceable.Objects.Remote.Composition.Service.Configuration;
    using Serviceable.Objects.Remote.Composition.ServiceOrchestrator;
    using TestHttpCompositionConsoleApp.Contexts.ConsoleLog;
    using TestHttpCompositionConsoleApp.Contexts.Http;
    using TestHttpCompositionConsoleApp.Contexts.Queues;

    class Program
    {
        static void Main(string[] args)
        {
            var serviceOrchestratorGraphTemplate = @"
{
    GraphNodes: [
        { TypeFullName:'" + typeof(ServiceOrchestratorContext).FullName + @"', Id:'server-orchestrator-context' },
        { TypeFullName:'" + typeof(NamedPipeServerContext).FullName + @"', Id:'named-pipe-instrumentation-context' },
        { TypeFullName:'" + typeof(InstrumentationServerContext).FullName + @"', Id:'instrumentation-context' },
    ],
    GraphVertices: [
        { FromId:'named-pipe-instrumentation-context', ToId:'instrumentation-context',  },
    ],
    Registrations: [
        { Type:'" + typeof(MemoryConfigurationSource).FullName + @"', WithDefaultInterface:true },
    ],
}
";

            var graphTemplate = @"
{
    GraphNodes: [
        { TypeFullName:'" + typeof(OwinHttpContext).FullName + @"', Id:'server-context' },
        { TypeFullName:'" + typeof(QueueContext).FullName + @"', Id:'queue-context' },
        { TypeFullName:'" + typeof(ConsoleLogContext).FullName + @"', Id:'console-log-context' },
        { TypeFullName:'" + typeof(NamedPipeServerContext).FullName + @"', Id:'namedpipes-log-instrumentation-context' },
        { TypeFullName:'" + typeof(NamedPipeServerContext).FullName + @"', Id:'namedpipes-instrumentation-context' },
        { TypeFullName:'" + typeof(InstrumentationServerContext).FullName + @"', Id:'instrumentation-context' },
    ],
    GraphVertices: [
        { FromId:'server-context', ToId:'queue-context', },
        { FromId:'queue-context', ToId:'console-log-context',  },
        { FromId:'namedpipes-log-instrumentation-context', ToId:'console-log-context',  },
        { FromId:'namedpipes-instrumentation-context', ToId:'instrumentation-context',  },
    ],
    Registrations: [
        { Type:'" + typeof(ServiceContainerConfigurationSource).FullName + @"', WithDefaultInterface:true },
    ],
}
";
            /*
             * Principles:
             * 
             * Composable
             * Configurable
             * Testable
             * Intrumentable
             * 
             */

            // TODO: Add process orchestrator

            // Start the service container
            var serviceOrchestratorContainer = new Container();
            var serviceOrchestratorGraph = new GraphContext(serviceOrchestratorContainer);
            serviceOrchestratorGraph.FromJson(serviceOrchestratorGraphTemplate);
            serviceOrchestratorGraph.Configure();
            serviceOrchestratorGraph.Setup();
            serviceOrchestratorGraph.Initialize();

            // TODO: should be a command
            var serviceOrchestratorContext = serviceOrchestratorContainer.Resolve<ServiceOrchestratorContext>();
            serviceOrchestratorContext.GraphTemplatesDictionary.Add("template-X", graphTemplate);
            serviceOrchestratorContext.ServiceRegistrations.Add(new ServiceRegistration { ServiceName = "service-X" });

            // Init the service container
            var service = new ServiceContext(new ServiceContextConfiguration
            {
                ServiceName = "service-X",
                OrchestratorName = "orchestrator-X",
                TemplateName = "template-X",
            });

            service.GraphContext.FromJson(graphTemplate);
            service.GraphContext.Configure();
            service.GraphContext.Setup();
            service.GraphContext.Initialize();

            new ApplicationHost().Execute(new RunAndBlock());
        }
    }
}