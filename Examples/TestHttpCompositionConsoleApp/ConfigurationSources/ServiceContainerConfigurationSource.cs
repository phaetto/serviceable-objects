namespace TestHttpCompositionConsoleApp.ConfigurationSources
{
    using System;
    using Newtonsoft.Json;
    using Serviceable.Objects.Composition.Graph.Stages.Configuration;
    using Serviceable.Objects.IO.NamedPipes.Server;
    using Serviceable.Objects.IO.NamedPipes.Server.Configuration;
    using Serviceable.Objects.Remote.Composition.ServiceContainer;
    using Serviceable.Objects.Remote.Composition.ServiceContainer.Configuration;

    public sealed class ServiceContainerConfigurationSource : IConfigurationSource
    {
        public string GetConfigurationValueForKey(string serviceContainerName, string serviceName, string graphNodeId, string typeName)
        {
            switch (typeName)
            {
                case var s when s == typeof(NamedPipeServerContext).FullName:
                    return JsonConvert.SerializeObject(new NamedPipeServerConfiguration
                    {
                        PipeName  = string.Join(".", serviceContainerName, serviceName ?? "self", "testpipe") // TODO: centralise the configuration discovery
                    });
                case var s when s == typeof(ServiceContainerContext).FullName:
                    return JsonConvert.SerializeObject(new ServiceContainerContextConfiguration
                    {
                        ContainerName = "container-X",
                        OrchestratorName = "orchestrator-X",
                    });
                default:
                    throw new InvalidOperationException($"Type {typeName} is not supported.");
            }
        }
    }
}
