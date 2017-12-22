namespace Serviceable.Objects.Remote.Composition.Service
{
    using System.Collections.Generic;
    using Composition.Configuration;
    using Configuration;
    using Objects.Composition.Graph;
    using Objects.Composition.Graph.Stages.Configuration;
    using Objects.Composition.Service;
    using Objects.Composition.ServiceOrchestrator;
    using Objects.Dependencies;

    public sealed class ServiceContext : ConfigurableContext<ServiceContextConfiguration, ServiceContext>, IService
    {
        public string OrchestratorName => Configuration.OrchestratorName;
        public string ServiceName => Configuration.ServiceName;
        public string TemplateName => Configuration.TemplateName;
        public IList<InBinding> InBindings => Configuration.InBindings;
        public IList<ExternalBinding> ExternalBindings => Configuration.ExternalBindings;
        public GraphContext GraphContext { get; }
        public Container ServiceContainer { get; } = new Container();

        public ServiceContext()
        {   
            var graphContainer = new Container(ServiceContainer);
            GraphContext = new GraphContext(graphContainer);
            ServiceContainer.Register(GraphContext);
            ServiceContainer.Register(typeof(IService), this);
        }

        public ServiceContext(ServiceContextConfiguration configuration) : base(configuration)
        {
            var graphContainer = new Container(ServiceContainer);
            GraphContext = new GraphContext(graphContainer);
            ServiceContainer.Register(GraphContext);
            ServiceContainer.Register(typeof(IService), this);
        }

        public ServiceContext(IConfigurationSource configurationSource) : base(configurationSource)
        {
            var graphContainer = new Container(ServiceContainer);
            GraphContext = new GraphContext(graphContainer);
            ServiceContainer.Register(GraphContext);
            ServiceContainer.Register(typeof(IService), this);
        }
    }
}
