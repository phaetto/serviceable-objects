namespace Serviceable.Objects.Composition.Graph.Stages.Configuration
{
    public interface IConfigurableStageFactory
    {
        bool HasBeenConfigured { get; }
        dynamic GenerateConfigurationCommand(string serializedConfigurationString);
    }
}