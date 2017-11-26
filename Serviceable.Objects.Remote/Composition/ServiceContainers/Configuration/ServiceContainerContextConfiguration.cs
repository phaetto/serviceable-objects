namespace Serviceable.Objects.Remote.Composition.ServiceContainers.Configuration
{
    using System.Collections.Generic;
    using Objects.Composition.ServiceContainers;

    public struct ServiceContainerContextConfiguration
    {
        public string ContainerName { get; set; }
        public Binding ServiceContainerBinding { get; set; }
        public IList<ExternalBinding> ExternalBindings { get; set; }
    }
}