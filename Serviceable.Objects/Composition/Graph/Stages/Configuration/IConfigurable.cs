namespace Serviceable.Objects.Composition.Graph.Stages.Configuration
{
    using ServiceContainers;
    using Services;

    public interface IConfigurable
    {
        bool HasBeenConfigured { get; }
        void Configure(IServiceContainer serviceContainer, IService service, ContextGraph contextGraph, ContextGraphNode contextGraphNode);
    }
}