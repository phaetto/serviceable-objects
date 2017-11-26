namespace Serviceable.Objects.Composition.Graph.Stages.Configuration
{
    using ServiceContainers;
    using Services;

    public interface IConfigurableStageFactory
    {
        bool HasBeenConfigured { get; }
        dynamic GenerateConfigurationCommand(IServiceContainer serviceContainer, IService service, GraphContext graphContext, GraphNodeContext graphNodeContext);
    }
}