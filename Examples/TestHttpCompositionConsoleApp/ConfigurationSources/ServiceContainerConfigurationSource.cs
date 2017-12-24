namespace TestHttpCompositionConsoleApp.ConfigurationSources
{
    using Contexts.Http;
    using Contexts.Http.Configuration;
    using Newtonsoft.Json;
    using Serviceable.Objects.Composition.Graph.Stages.Configuration;
    using Serviceable.Objects.IO.NamedPipes.Server;
    using Serviceable.Objects.IO.NamedPipes.Server.Configuration;

    public sealed class ServiceContainerConfigurationSource : IConfigurationSource
    {
        public string GetConfigurationValueForKey(string serviceName, string graphNodeId, string typeName)
        {
            switch (typeName)
            {
                case var s when s == typeof(NamedPipeServerContext).AssemblyQualifiedName:
                    return JsonConvert.SerializeObject(new NamedPipeServerConfiguration
                    {
                        PipeName  = string.Join(".", serviceName, graphNodeId) // TODO: centralise the configuration discovery
                    });
                case var s when s == typeof(OwinHttpContext).AssemblyQualifiedName:
                    return JsonConvert.SerializeObject(new OwinHttpContextConfiguration()
                    {
                        Host = "$in.Host",
                        Port = "$in.Port",
                    });
                default:
                    return null;
            }
        }
    }
}
