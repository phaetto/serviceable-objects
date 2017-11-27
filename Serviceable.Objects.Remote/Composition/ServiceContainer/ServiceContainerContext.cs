namespace Serviceable.Objects.Remote.Composition.ServiceContainer
{
    using System.Collections.Generic;
    using Composition.Configuration;
    using Configuration;
    using Objects.Composition.Graph;
    using Objects.Composition.Graph.Stages.Configuration;
    using Objects.Composition.Service;
    using Objects.Composition.ServiceContainer;
    using Objects.Dependencies;

    public sealed class ServiceContainerContext : ConfigurableContext<ServiceContainerContextConfiguration, ServiceContainerContext>, IServiceContainer
    {
        public readonly GraphContext GraphContext;
        public string OrchestratorName => Configuration.OrchestratorName;
        public string ContainerName => Configuration.ContainerName;
        public IList<IService> Services { get; } = new List<IService>();
        public Container ServiceContainerContextContainer { get; }

        public ServiceContainerContext(GraphContext graphContext)
        {
            this.GraphContext = graphContext;
            ServiceContainerContextContainer = new Container(graphContext.Container);
            graphContext.Container.Register(typeof(IServiceContainer), this);
        }

        public ServiceContainerContext(ServiceContainerContextConfiguration configuration, GraphContext graphContext) : base(configuration)
        {
            this.GraphContext = graphContext;
            ServiceContainerContextContainer = new Container(graphContext.Container);
            graphContext.Container.Register(typeof(IServiceContainer), this);
        }

        public ServiceContainerContext(IConfigurationSource configurationSource, GraphContext graphContext) : base(configurationSource)
        {
            this.GraphContext = graphContext;
            ServiceContainerContextContainer = new Container(graphContext.Container);
            graphContext.Container.Register(typeof(IServiceContainer), this);
        }
    }
}
