namespace Serviceable.Objects.Composition.Graph.Stages.Configuration
{
    using System;
    using ServiceContainers;
    using Services;

    public interface IConfigurationSource
    {
        string GetConfigurationValueForKey(IServiceContainer serviceContainer, IService service, GraphContext graphContext, GraphNodeContext graphNodeContext, Type type);
    }
}