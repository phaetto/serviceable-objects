namespace Serviceable.Objects.Composition.Stages.Configuration
{
    public interface IConfigurable
    {
        bool HasBeenConfigured { get; }
        void Configure(ContextGraph contextGraph, ContextGraphNode contextGraphNode);
    }
}