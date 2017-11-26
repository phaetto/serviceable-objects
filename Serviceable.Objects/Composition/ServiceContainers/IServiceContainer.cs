namespace Serviceable.Objects.Composition.ServiceContainers
{
    using System.Collections.Generic;
    using Dependencies;
    using Services;

    public interface IServiceContainer
    {
        IList<IService> Services { get; }
        string ServiceName { get; }
        Container ServiceContainerContainer { get; }
        Binding ServiceContainerBinding { get; }
        IList<ExternalBinding> ExternalBindings { get; }
    }
}