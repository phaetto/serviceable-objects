
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
    using Contexts.ConsoleLog;
    using Contexts.Http;
    using Contexts.Queues;

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
        { Type:'" + typeof(MemoryConfigurationSource).AssemblyQualifiedName + @"', WithDefaultInterface:true },
    ],
}
";

            var graphTemplate = @"
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
             * Intrumentable
             * Scalable TODO
             * Updatable TODO
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