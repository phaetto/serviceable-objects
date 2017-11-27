namespace TestHttpCompositionConsoleApp.ConfigurationSources
{
    using System;
    using Newtonsoft.Json;
    using Serviceable.Objects.Composition.Graph;
    using Serviceable.Objects.Composition.Graph.Stages.Configuration;
    using Serviceable.Objects.Composition.Service;
    using Serviceable.Objects.Composition.ServiceContainer;
    using Serviceable.Objects.IO.NamedPipes.Server;
    using Serviceable.Objects.IO.NamedPipes.Server.Configuration;
    using Serviceable.Objects.Remote.Composition.ServiceContainer;
    using Serviceable.Objects.Remote.Composition.ServiceContainer.Configuration;

    public sealed class ServiceContainerConfigurationSource : IConfigurationSource
    {
        public string GetConfigurationValueForKey(IServiceContainer serviceContainer, IService service, GraphContext graphContext, GraphNodeContext graphNodeContext, Type type)
        {
            switch (type)
            {
                case var t when t == typeof(NamedPipeServerContext):
                    return JsonConvert.SerializeObject(new NamedPipeServerConfiguration
                    {
                        PipeName  = string.Join(".", serviceContainer?.ContainerName, service?.ServiceName ?? "self", "testpipe")
                    });
                case var t when t == typeof(ServiceContainerContext):
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
