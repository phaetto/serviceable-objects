
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
    using Serviceable.Objects.Remote.Composition.ServiceContainer;
    using Serviceable.Objects.Remote.Composition.ServiceContainer.Commands;
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
        { TypeFullName:'" + typeof(InstrumentationServer).FullName + @"', Id:'instrumentation-context' },
    ],
    GraphVertices: [
        { FromId:'named-pipe-instrumentation-context', ToId:'instrumentation-context',  },
    ],
}
";

            var serviceContainerGraphTemplate = @"
{
    GraphNodes: [
        { TypeFullName:'" + typeof(ServiceContainerContext).FullName + @"', Id:'server-container-context' },
        { TypeFullName:'" + typeof(NamedPipeServerContext).FullName + @"', Id:'named-pipe-instrumentation-context' },
        { TypeFullName:'" + typeof(InstrumentationServer).FullName + @"', Id:'instrumentation-context' },
    ],
    GraphVertices: [
        { FromId:'named-pipe-instrumentation-context', ToId:'instrumentation-context',  },
    ],
}
";

            var graphTemplate = @"
{
    GraphNodes: [
        { TypeFullName:'" + typeof(OwinHttpContext).FullName + @"', Id:'server-context' },
        { TypeFullName:'" + typeof(QueueContext).FullName + @"', Id:'queue-context' },
        { TypeFullName:'" + typeof(ConsoleLogContext).FullName + @"', Id:'log-context' },
        { TypeFullName:'" + typeof(NamedPipeServerContext).FullName + @"', Id:'console-instrumentation-context' },
    ],
    GraphVertices: [
        { FromId:'server-context', ToId:'queue-context', },
        { FromId:'queue-context', ToId:'log-context',  },
        { FromId:'console-instrumentation-context', ToId:'log-context',  },
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
            serviceOrchestratorContainer.RegisterWithDefaultInterface(typeof(MemoryConfigurationSource)); // TODO: Move this to graph configuration
            var serviceOrchestratorrGraph = new GraphContext(serviceOrchestratorContainer);
            serviceOrchestratorrGraph.FromJson(serviceOrchestratorGraphTemplate);
            serviceOrchestratorrGraph.Configure();
            serviceOrchestratorrGraph.Initialize();

            // TODO: should be a command
            var serviceOrchestratorContext = serviceOrchestratorContainer.Resolve<ServiceOrchestratorContext>();
            serviceOrchestratorContext.GraphTemplatesDictionary.Add("template-X", graphTemplate);
            serviceOrchestratorContext.ServiceRegistrations.Add(new ServiceRegistration { ServiceName = "service-X" });

            // Init the service container
            var container = new Container(); // Move this to graph configuration
            container.RegisterWithDefaultInterface(typeof(ServiceContainerConfigurationSource)); // TODO: Move this to graph configuration
            var serviceContainerGraph = new GraphContext(container);
            serviceContainerGraph.FromJson(serviceContainerGraphTemplate);
            serviceContainerGraph.Configure();
            serviceContainerGraph.Initialize();

            serviceContainerGraph.Execute(new StartServiceWithTemplate(
                serviceOrchestratorContext.GraphTemplatesDictionary["template-X"],
                "template-X",
                "service-X"
            ));

            new ApplicationHost().Execute(new RunAndBlock());
        }
    }
}