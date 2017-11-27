namespace Serviceable.Objects.Composition.ServiceContainer
{
    using System.Collections.Generic;
    using Dependencies;
    using Service;

    public interface IServiceContainer
    {
        string ContainerName { get; }
        Container ServiceContainerContextContainer { get; }
        IList<IService> Services { get; }
    }
}