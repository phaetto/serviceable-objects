namespace Serviceable.Objects.Composition.Graph.Stages.Configuration
{
    using Service;
    using ServiceContainer;

    public interface IConfigurableStageFactory
    {
        bool HasBeenConfigured { get; }
        dynamic GenerateConfigurationCommand(IServiceContainer serviceContainer, IService service, GraphContext graphContext, GraphNodeContext graphNodeContext);
    }
}