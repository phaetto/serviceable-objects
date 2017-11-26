namespace Serviceable.Objects.Remote.Composition.ServiceContainers
{
    using System.Collections.Generic;
    using Composition.Configuration;
    using Configuration;
    using Objects.Composition.ServiceContainers;
    using Objects.Dependencies;
    using Serviceable.Objects.Composition.Graph.Stages.Configuration;

    public sealed class ServiceContainerContext : ConfigurableContext<ServiceContainerContextConfiguration, ServiceContainerContext>, IServiceContainer
    {
        public string ContainerName => Configuration.ContainerName;
        public Binding ServiceContainerBinding => Configuration.ServiceContainerBinding;
        public IList<ExternalBinding> ExternalBindings => Configuration.ExternalBindings;
        public IList<ServiceRegistration> ServiceRegistrations { get; } = new List<ServiceRegistration>();
        public Container ServiceContainerContainer { get; } = new Container();
        public IDictionary<string, string> GraphTemplatesDictionary { get; } = new Dictionary<string, string>();

        public ServiceContainerContext()
        {
            ServiceContainerContainer.Register(typeof(IServiceContainer), this);
        }

        public ServiceContainerContext(ServiceContainerContextConfiguration configuration) : base(configuration)
        {
            ServiceContainerContainer.Register(typeof(IServiceContainer), this);
        }

        public ServiceContainerContext(IConfigurationSource configurationSource) : base(configurationSource)
        {
            ServiceContainerContainer.Register(typeof(IServiceContainer), this);
        }
    }
}