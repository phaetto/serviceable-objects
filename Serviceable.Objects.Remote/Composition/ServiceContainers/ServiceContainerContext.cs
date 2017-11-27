namespace Serviceable.Objects.Remote.Composition.ServiceContainers
{
    using System.Collections.Generic;
    using Composition.Configuration;
    using Configuration;
    using Objects.Composition.Graph.Stages.Configuration;
    using Objects.Composition.ServiceContainers;
    using Objects.Composition.Services;
    using Objects.Dependencies;
    using Services;

    public sealed class ServiceContainerContext : ConfigurableContext<ServiceContainerContextConfiguration, ServiceContext>, IServiceContainer
    {
        public string OrchestratorName => Configuration.OrchestratorName;
        public string ContainerName => Configuration.ContainerName;
        public IList<IService> Services { get; } = new List<IService>();
        public Container ServiceContainerContextContainer { get; } = new Container();

        public ServiceContainerContext()
        {   
            ServiceContainerContextContainer.Register(typeof(IServiceContainer), this);
        }

        public ServiceContainerContext(ServiceContainerContextConfiguration configuration) : base(configuration)
        {
            ServiceContainerContextContainer.Register(typeof(IServiceContainer), this);
        }

        public ServiceContainerContext(IConfigurationSource configurationSource) : base(configurationSource)
        {
            ServiceContainerContextContainer.Register(typeof(IServiceContainer), this);
        }
    }
}
