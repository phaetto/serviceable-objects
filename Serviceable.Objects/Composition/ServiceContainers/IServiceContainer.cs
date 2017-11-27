namespace Serviceable.Objects.Composition.ServiceContainers
{
    using System.Collections.Generic;
    using Dependencies;
    using Services;

    public interface IServiceContainer
    {
        string ContainerName { get; }
        Container ServiceContainerContextContainer { get; }
        IList<IService> Services { get; }
    }
}