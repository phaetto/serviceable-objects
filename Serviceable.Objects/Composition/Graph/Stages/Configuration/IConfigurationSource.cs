namespace Serviceable.Objects.Composition.Graph.Stages.Configuration
{
    public interface IConfigurationSource
    {
        string GetConfigurationValueForKey(string serviceName, string graphNodeId, string typeName);
    }
}