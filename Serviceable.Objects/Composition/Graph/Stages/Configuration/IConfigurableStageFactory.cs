namespace Serviceable.Objects.Composition.Graph.Stages.Configuration
{
    using Service;

    public interface IConfigurableStageFactory
    {
        bool HasBeenConfigured { get; }
        dynamic GenerateConfigurationCommand(IService service, GraphContext graphContext, GraphNodeContext graphNodeContext);
    }
}