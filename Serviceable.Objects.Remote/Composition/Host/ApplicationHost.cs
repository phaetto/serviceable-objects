namespace Serviceable.Objects.Remote.Composition.Host
{
    using System.Threading;
    using Configuration;
    using Dependencies;
    using Exceptions;
    using Graph;
    using Newtonsoft.Json;
    using Objects.Composition.Graph;
    using Objects.Composition.Service;
    using Objects.Composition.ServiceOrchestrator;
    using Objects.Dependencies;
    using Service;
    using ServiceOrchestrator;

    public sealed class ApplicationHost : Context<ApplicationHost> // TODO: consider moving it to IO
    {
        public readonly string DefaultOrchestratorTemplate = @"
{
    GraphNodes: [
        { TypeFullName:'" + typeof(ServiceOrchestratorContext).AssemblyQualifiedName + @"', Id:'server-orchestrator-context' },
    ],
    GraphVertices: [
    ],
    Registrations: [
        { Type:'" + typeof(ServiceOrchestratorDefaultConfigurationSource).AssemblyQualifiedName + @"', WithDefaultInterface:true },
    ],
}
";

        public readonly GraphContext GraphContext;
        public readonly IService Service;
        public readonly IServiceOrchestrator ServiceOrchestrator;
        public EventWaitHandle EventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        public CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        public ApplicationHost(IServiceOrchestrator serviceOrchestrator, GraphContext graphContext)
        {
            ServiceOrchestrator = serviceOrchestrator;
            serviceOrchestrator.ServiceOrchestratorContainer.Register(this);
            GraphContext = graphContext;
        }

        public ApplicationHost(IService service)
        {
            Service = service;
            service.ServiceContainer.Register(this);
            GraphContext = Service.GraphContext;
        }

        public ApplicationHost(string[] args) : this(string.Join(" ", args))
        {
        }

        public ApplicationHost(string jsonString)
        {
            // TODO: Add console integration (debug print, redirection etc)
            var configuration = JsonConvert.DeserializeObject<ApplicationHostDataConfiguration>(jsonString);
            if (!string.IsNullOrWhiteSpace(configuration.ServiceContextConfiguration.ServiceName))
            {
                // This is a service
                Service = new ServiceContext(configuration.ServiceContextConfiguration);
                GraphContext = Service.GraphContext;
                if (configuration.DependencyInjectionRegistrationTemplate != null)
                {
                    GraphContext.Container.From(configuration.DependencyInjectionRegistrationTemplate);
                }
                Check.ArgumentNull(configuration.ServiceGraphTemplate, nameof(configuration.ServiceGraphTemplate), "Service template cannot be empty.");
                GraphContext.From(configuration.ServiceGraphTemplate);
            }
            else
            {
                // This is an orchestrator
                var container = new Container();
                container.Register(new ServiceOrchestratorDefaultConfigurationSource(configuration.ServiceOrchestratorConfiguration));
                GraphContext = new GraphContext(container);
                if (configuration.OrchestratorOverrideTemplate != null)
                {
                    if (configuration.DependencyInjectionRegistrationTemplate != null)
                    {
                        GraphContext.Container.From(configuration.DependencyInjectionRegistrationTemplate);
                    }
                    GraphContext.From(configuration.OrchestratorOverrideTemplate);
                }
                else
                {
                    GraphContext.Container.FromJson(DefaultOrchestratorTemplate);
                    GraphContext.FromJson(DefaultOrchestratorTemplate);
                }
            }
        }
    }
}