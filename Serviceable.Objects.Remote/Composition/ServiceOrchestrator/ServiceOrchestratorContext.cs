namespace Serviceable.Objects.Remote.Composition.ServiceOrchestrator
{
    using System.Collections.Generic;
    using Composition.Configuration;
    using Configuration;
    using Objects.Composition.Graph;
    using Objects.Composition.Graph.Stages.Configuration;
    using Objects.Composition.ServiceOrchestrator;
    using Objects.Dependencies;

    public sealed class ServiceOrchestratorContext : ConfigurableContext<ServiceOrchestratorConfiguration, ServiceOrchestratorContext>, IServiceOrchestrator
    {
        public string OrchestratorName => Configuration.OrchestratorName;
        public Binding Binding => Configuration.ServiceOrchestratorBinding;
        public ExternalBinding ExternalBinding => Configuration.ExternalBinding;
        public IList<ServiceRegistration> ServiceRegistrations { get; } = new List<ServiceRegistration>();
        public Container ServiceOrchestratorContainer { get; } = new Container();
        public IDictionary<string, string> GraphTemplatesDictionary { get; } = new Dictionary<string, string>();

        public ServiceOrchestratorContext(GraphContext graphContext)
        {
            graphContext.Container.Register(typeof(IServiceOrchestrator), this);
        }

        public ServiceOrchestratorContext(ServiceOrchestratorConfiguration configuration, GraphContext graphContext) : base(configuration)
        {
            graphContext.Container.Register(typeof(IServiceOrchestrator), this);
        }

        public ServiceOrchestratorContext(IConfigurationSource configurationSource, GraphContext graphContext) : base(configurationSource)
        {
            graphContext.Container.Register(typeof(IServiceOrchestrator), this);
        }
    }
}