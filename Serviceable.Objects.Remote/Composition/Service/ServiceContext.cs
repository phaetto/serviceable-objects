namespace Serviceable.Objects.Remote.Composition.Service
{
    using Composition.Configuration;
    using Configuration;
    using Objects.Composition.Graph;
    using Objects.Composition.Graph.Stages.Configuration;
    using Objects.Composition.Service;
    using Objects.Dependencies;

    public sealed class ServiceContext : ConfigurableContext<ServiceContextConfiguration, ServiceContext>, IService
    {
        public string ContainerName => Configuration.ContainerName;
        public string ServiceName => Configuration.ServiceName;
        public string TemplateName => Configuration.TemplateName;
        public GraphContext GraphContext { get; }
        public Container ServiceContainer { get; }

        public ServiceContext(Container serviceContainerContextContainer = null)
        {   
            ServiceContainer = new Container(serviceContainerContextContainer);
            var graphContainer = new Container(ServiceContainer);
            GraphContext = new GraphContext(graphContainer);
            ServiceContainer.Register(GraphContext);
            ServiceContainer.Register(typeof(IService), this);
        }

        public ServiceContext(ServiceContextConfiguration configuration, Container serviceContainerContextContainer = null) : base(configuration)
        {
            ServiceContainer = new Container(serviceContainerContextContainer);
            var graphContainer = new Container(ServiceContainer);
            GraphContext = new GraphContext(graphContainer);
            ServiceContainer.Register(GraphContext);
            ServiceContainer.Register(typeof(IService), this);
        }

        public ServiceContext(IConfigurationSource configurationSource, Container serviceContainerContextContainer = null) : base(configurationSource)
        {
            ServiceContainer = new Container(serviceContainerContextContainer);
            var graphContainer = new Container(ServiceContainer);
            GraphContext = new GraphContext(graphContainer);
            ServiceContainer.Register(GraphContext);
            ServiceContainer.Register(typeof(IService), this);
        }
    }
}
