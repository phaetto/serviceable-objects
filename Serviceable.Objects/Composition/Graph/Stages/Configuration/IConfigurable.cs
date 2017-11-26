namespace Serviceable.Objects.Composition.Graph.Stages.Configuration
{
    public interface IConfigurable
    {
        bool HasBeenConfigured { get; }
        void Configure(ContextGraph contextGraph, ContextGraphNode contextGraphNode);
    }
}