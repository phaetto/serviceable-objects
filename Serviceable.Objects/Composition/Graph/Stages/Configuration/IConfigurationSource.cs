namespace Serviceable.Objects.Composition.Graph.Stages.Configuration
{
    using System;
    using Service;
    using ServiceContainer;

    public interface IConfigurationSource
    {
        string GetConfigurationValueForKey(IServiceContainer serviceContainer, IService service, GraphContext graphContext, GraphNodeContext graphNodeContext, Type type);
    }
}