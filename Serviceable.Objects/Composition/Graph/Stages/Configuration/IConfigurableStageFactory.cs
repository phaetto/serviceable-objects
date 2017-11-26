namespace Serviceable.Objects.Composition.Graph.Stages.Configuration
{
    using Services;

    public interface IConfigurableStageFactory
    {
        bool HasBeenConfigured { get; }
        dynamic GenerateConfigurationCommand(IService service, GraphContext graphContext, GraphNodeContext graphNodeContext);
    }
}