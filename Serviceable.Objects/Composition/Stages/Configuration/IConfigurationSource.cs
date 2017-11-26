namespace Serviceable.Objects.Composition.Stages.Configuration
{
    using System;

    public interface IConfigurationSource
    {
        // TODO: service template type
        // TODO: container bindings
        string GetConfigurationValueForKey(ContextGraph contextGraph, ContextGraphNode contextGraphNode, Type type);
    }
}