namespace Serviceable.Objects.Remote.Composition.ServiceOrchestrator
{
    using System.Collections.Generic;
    using Composition.Configuration;
    using Configuration;
    using Objects.Composition.Graph.Stages.Configuration;
    using Objects.Composition.ServiceOrchestrator;
    using Objects.Dependencies;

    public sealed class ServiceOrchestratorContext : ConfigurableContext<ServiceOrchestratorConfiguration, ServiceOrchestratorContext>, IServiceOrchestrator
    {
        public string OrchestratorName => Configuration.OrchestratorName;
        public Binding ServiceOrchestratorBinding => Configuration.ServiceOrchestratorBinding;
        public IList<ExternalBinding> ExternalBindings => Configuration.ExternalBindings;
        public IList<ServiceRegistration> ServiceRegistrations { get; } = new List<ServiceRegistration>();
        public Container ServiceOrchestratorContainer { get; } = new Container();
        public IDictionary<string, string> GraphTemplatesDictionary { get; } = new Dictionary<string, string>();

        public ServiceOrchestratorContext()
        {
            ServiceOrchestratorContainer.Register(typeof(IServiceOrchestrator), this);
        }

        public ServiceOrchestratorContext(ServiceOrchestratorConfiguration configuration) : base(configuration)
        {
            ServiceOrchestratorContainer.Register(typeof(IServiceOrchestrator), this);
        }

        public ServiceOrchestratorContext(IConfigurationSource configurationSource) : base(configurationSource)
        {
            ServiceOrchestratorContainer.Register(typeof(IServiceOrchestrator), this);
        }
    }
}