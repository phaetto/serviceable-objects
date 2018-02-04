namespace Serviceable.Objects.Remote.Composition.ServiceOrchestrator
{
    using System.Collections.Generic;
    using Composition.Configuration;
    using Configuration;
    using Objects.Composition.Graph;
    using Objects.Composition.ServiceOrchestrator;
    using Objects.Dependencies;

    public sealed class ServiceOrchestratorContext : ConfigurableContext<ServiceOrchestratorConfiguration, ServiceOrchestratorContext>, IServiceOrchestrator
    {
        public Container ServiceOrchestratorContainer { get; }
        public IList<ServiceRegistration> ServiceRegistrations { get; } = new List<ServiceRegistration>(); // TODO: evaluate if needed

        public string OrchestratorName => Configuration.OrchestratorName;
        public string EntryAssemblyFullPath => Configuration.EntryAssemblyFullPath;
        public bool UseChildProcesses => Configuration.UseChildProcesses;
        public IDictionary<string, List<InBinding>> InBindingsPerService => Configuration.InBindingsPerService;
        public IDictionary<string, List<ExternalBinding>> ExternalBindingsPerService => Configuration.ExternalBindingsPerService;
        public IDictionary<string, string> GraphTemplatesDictionary => Configuration.GraphTemplatesDictionary;

        public ServiceOrchestratorContext(GraphContext graphContext)
        {
            ServiceOrchestratorContainer = new Container(graphContext.Container);
            graphContext.Container.Register(typeof(IServiceOrchestrator), this);
        }

        public ServiceOrchestratorContext(ServiceOrchestratorConfiguration configuration, GraphContext graphContext) : base(configuration)
        {
            ServiceOrchestratorContainer = new Container(graphContext.Container);
            graphContext.Container.Register(typeof(IServiceOrchestrator), this);
        }
    }
}