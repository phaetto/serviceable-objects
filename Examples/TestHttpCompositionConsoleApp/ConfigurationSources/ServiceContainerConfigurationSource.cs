namespace TestHttpCompositionConsoleApp.ConfigurationSources
{
    using System;
    using Newtonsoft.Json;
    using Serviceable.Objects.Composition.Graph;
    using Serviceable.Objects.Composition.Graph.Stages.Configuration;
    using Serviceable.Objects.Composition.Service;
    using Serviceable.Objects.IO.NamedPipes.Server;
    using Serviceable.Objects.IO.NamedPipes.Server.Configuration;
    using Serviceable.Objects.Remote.Composition.ServiceContainer.Configuration;

    public sealed class ServiceContainerConfigurationSource : IConfigurationSource
    {
        public string GetConfigurationValueForKey(IService service, GraphContext graphContext, GraphNodeContext graphNodeContext, Type type)
        {
            switch (type)
            {
                case var t when t == typeof(NamedPipeServerContext):
                    return JsonConvert.SerializeObject(new NamedPipeServerConfiguration
                    {
                        PipeName  = "testpipe"
                    });
                case var t when t == typeof(ServiceContainerContextConfiguration):
                    return JsonConvert.SerializeObject(new ServiceContainerContextConfiguration
                    {
                        ContainerName = "container-X",
                        OrchestratorName = "orchestrator-X",
                    });
                default:
                    throw new InvalidOperationException($"Type {type.FullName} is not supported.");
            }
        }
    }
}
