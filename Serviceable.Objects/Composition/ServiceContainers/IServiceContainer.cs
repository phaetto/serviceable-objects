namespace Serviceable.Objects.Composition.ServiceContainers
{
    using System.Collections.Generic;
    using Dependencies;
    using Services;

    public interface IServiceContainer
    {
        IList<ServiceRegistration> ServiceRegistrations { get; }
        string ContainerName { get; }
        Container ServiceContainerContainer { get; }
        Binding ServiceContainerBinding { get; }
        IList<ExternalBinding> ExternalBindings { get; }
        IDictionary<string, string> GraphTemplatesDictionary { get; }
    }
}