namespace Serviceable.Objects.Remote.Composition.Host
{
    using System;
    using System.Threading;
    using Configuration;
    using Dependencies;
    using Exceptions;
    using Graph;
    using Newtonsoft.Json;
    using Objects.Composition.Graph;
    using Objects.Composition.Service;
    using Objects.Composition.ServiceOrchestrator;
    using Service;
    using ServiceOrchestrator;

    public sealed class ApplicationHost : Context<ApplicationHost>
    {
        public readonly string DefaultOrchestratorTemplate = @"
{
    GraphNodes: [
        { TypeFullName:'" + typeof(ServiceOrchestratorContext).AssemblyQualifiedName + @"', Id:'service-orchestrator-context' },
    ],
    GraphVertices: [
    ],
    Registrations: [
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
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                throw new InvalidOperationException("Configuration json must be provided");
            }

            var configuration = JsonConvert.DeserializeObject<ApplicationHostDataConfiguration>(jsonString);

            if (!string.IsNullOrWhiteSpace(configuration.ServiceContextConfiguration.ServiceName))
            {
                // This is a service
                Service = new ServiceContext(configuration.ServiceContextConfiguration);
                Service.ServiceContainer.Register(this);
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
                Service = new ServiceContext(configuration.ServiceContextConfiguration);
                Service.ServiceContainer.Register(this);
                Service.ServiceContainer.RegisterWithDefaultInterface(new ServiceOrchestratorDefaultConfigurationSource(configuration.ServiceOrchestratorConfiguration));
                GraphContext = Service.GraphContext;
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