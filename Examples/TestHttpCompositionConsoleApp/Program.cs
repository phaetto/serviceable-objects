
namespace TestHttpCompositionConsoleApp
{
    using System.Collections.Generic;
    using Contexts.ConfigurationSource;
    using Serviceable.Objects.Composition.ServiceOrchestrator;
    using Serviceable.Objects.IO.NamedPipes.Server;
    using Serviceable.Objects.Remote.Composition.Graph;
    using Serviceable.Objects.Remote.Composition.ServiceContainers;
    using Serviceable.Objects.Remote.Composition.ServiceContainers.Configuration;
    using Serviceable.Objects.Remote.Composition.ServiceOrchestrator;
    using Serviceable.Objects.Remote.Composition.ServiceOrchestrator.Configuration;
    using Serviceable.Objects.Remote.Composition.Services;
    using Serviceable.Objects.Remote.Composition.Services.Configuration;
    using TestHttpCompositionConsoleApp.Contexts.ConsoleLog;
    using TestHttpCompositionConsoleApp.Contexts.Http;
    using TestHttpCompositionConsoleApp.Contexts.Http.Commands;
    using TestHttpCompositionConsoleApp.Contexts.Queues;

    class Program
    {
        static void Main(string[] args)
        {
            var graphTemplate = @"
{
    GraphNodes: [
        { TypeFullName:'" + typeof(OwinHttpContext).FullName + @"', Id:'server-context' },
        { TypeFullName:'" + typeof(QueueContext).FullName + @"', Id:'queue-context' },
        { TypeFullName:'" + typeof(ConsoleLogContext).FullName + @"', Id:'log-context' },
        { TypeFullName:'" + typeof(NamedPipeServerContext).FullName + @"', Id:'named-pipe-instrumentation-context' },
        /*{ TypeFullName:'" + typeof(NamedPipeServerContext).FullName + @"', Id:'instrumentation-context' },*/
    ],
    GraphVertices: [
        { FromId:'server-context', ToId:'queue-context', },
        { FromId:'queue-context', ToId:'log-context',  },
        { FromId:'named-pipe-instrumentation-context', ToId:'log-context',  },
        /*{ FromId:'named-pipe-instrumentation-context', ToId:'instrumentation-context',  },*/
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

            // TODO: Add host for service-containers (bring-your-own-process logic)
            // TODO: Add process orchestrator

            // Start the service container
            var serviceOrchestratorContext = new ServiceOrchestratorContext(new ServiceOrchestratorConfiguration
            {
                OrchestratorName = "service-container",
                ServiceOrchestratorBinding = new Binding { Host = "localhost" },
                ExternalBindings = new List<ExternalBinding>(),
            });
            serviceOrchestratorContext.GraphTemplatesDictionary.Add("template-X", graphTemplate);
            serviceOrchestratorContext.ServiceRegistrations.Add(new ServiceRegistration { ServiceName = "service-X" });
            serviceOrchestratorContext.ServiceOrchestratorContainer.RegisterWithDefaultInterface(typeof(MemoryConfigurationSource));

            // Init the service container
            var serviceContainer = new ServiceContainerContext(new ServiceContainerContextConfiguration
            {
                OrchestratorName = "service-container",
                ContainerName = "container-X",
            });
            serviceContainer.ServiceContainerContextContainer.RegisterWithDefaultInterface(typeof(MemoryConfigurationSource));

            // Start the service // TODO: needs to be a command on ServiceContainerContext
            var service = new ServiceContext(
                new ServiceContextConfiguration
                {
                    ContainerName = "container-X",
                    ServiceName = "service-X",
                    TemplateName = "template-X",
                },
                serviceContainer.ServiceContainerContextContainer
            );

            // TODO: instrumentation service:
            // TODO: Named-pipes -> service (template) ---> merge with graph/service template

            service.GraphContext.FromJson(graphTemplate);
            service.GraphContext.Configure();
            service.GraphContext.Initialize();
            service.GraphContext.Execute(new Run()); // TODO: blocking operation must be on Host - owin must run on a task
        }
    }
}