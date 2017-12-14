namespace TestHttpCompositionConsoleApp.ConfigurationSources
{
    using System;
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
                default:
                    return null;
            }
        }
    }
}
