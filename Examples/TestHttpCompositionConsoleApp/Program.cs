
namespace TestHttpCompositionConsoleApp
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ConfigurationSources;
    using Serviceable.Objects.Instrumentation.Server;
    using Serviceable.Objects.IO.NamedPipes.Server;
    using Serviceable.Objects.Remote.Composition.Host;
    using Serviceable.Objects.Remote.Composition.Host.Commands;
    using Serviceable.Objects.Remote.Composition.Service.Configuration;
    using Serviceable.Objects.Remote.Composition.ServiceOrchestrator;
    using Contexts.ConsoleLog;
    using Contexts.Http;
    using Contexts.Queues;
    using Newtonsoft.Json;
    using Serviceable.Objects.Composition.ServiceOrchestrator;
    using Serviceable.Objects.Remote.Composition.Graph;
    using Serviceable.Objects.Remote.Composition.Host.Configuration;
    using Serviceable.Objects.Remote.Composition.ServiceOrchestrator.Configuration;
    using Serviceable.Objects.Remote.Dependencies;

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

            // TODO: Add process orchestrator

            //// TODO: should be a command
            //var serviceOrchestratorContext = serviceOrchestratorContainer.Resolve<ServiceOrchestratorContext>();
            //serviceOrchestratorContext.GraphTemplatesDictionary.Add("template-X", graphTemplate);
            //serviceOrchestratorContext.ServiceRegistrations.Add(new ServiceRegistration { ServiceName = "service-X" });

            var serviceOrchestratorConfiguration = new ServiceOrchestratorConfiguration
            {
                OrchestratorName = "orchestrator-X",
            };

            Task.Run(() =>
            {
                new ApplicationHost(JsonConvert.SerializeObject(new ApplicationHostDataConfiguration
                {
                    DependencyInjectionRegistrationTemplate = JsonConvert.DeserializeObject<DependencyInjectionRegistrationTemplate>(serviceOrchestratorGraphTemplate),
                    OrchestratorOverrideTemplate = JsonConvert.DeserializeObject<GraphTemplate>(serviceOrchestratorGraphTemplate),
                    ServiceOrchestratorConfiguration = serviceOrchestratorConfiguration,
                }))
                .Execute(new RunAndBlock());
            });

            var serviceContextConfiguration = new ServiceContextConfiguration
            {
                ServiceName = "service-X",
                OrchestratorName = "orchestrator-X",
                TemplateName = "template-X",
                InBindings = new List<InBinding>
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
            };

            new ApplicationHost(JsonConvert.SerializeObject(new ApplicationHostDataConfiguration
            {
                ServiceOrchestratorConfiguration = serviceOrchestratorConfiguration,
                ServiceContextConfiguration = serviceContextConfiguration,
                DependencyInjectionRegistrationTemplate = JsonConvert.DeserializeObject<DependencyInjectionRegistrationTemplate>(serviceGraphTemplate),
                ServiceGraphTemplate = JsonConvert.DeserializeObject<GraphTemplate>(serviceGraphTemplate),
            }))
            .Execute(new RunAndBlock());
        }
    }
}