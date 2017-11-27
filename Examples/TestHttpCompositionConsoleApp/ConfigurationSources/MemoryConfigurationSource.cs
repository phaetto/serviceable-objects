namespace TestHttpCompositionConsoleApp.ConfigurationSources
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Serviceable.Objects.Composition.Graph;
    using Serviceable.Objects.Composition.Graph.Stages.Configuration;
    using Serviceable.Objects.Composition.Service;
    using Serviceable.Objects.Composition.ServiceContainer;
    using Serviceable.Objects.Composition.ServiceOrchestrator;
    using Serviceable.Objects.IO.NamedPipes.Server;
    using Serviceable.Objects.IO.NamedPipes.Server.Configuration;
    using Serviceable.Objects.Remote.Composition.ServiceOrchestrator;
    using Serviceable.Objects.Remote.Composition.ServiceOrchestrator.Configuration;

    public sealed class MemoryConfigurationSource : IConfigurationSource
    {
        public string GetConfigurationValueForKey(IServiceContainer serviceContainer, IService service, GraphContext graphContext, GraphNodeContext graphNodeContext, Type type)
        {
            switch (type)
            {
                case var t when t == typeof(NamedPipeServerContext):
                    return JsonConvert.SerializeObject(new NamedPipeServerConfiguration
                    {
                        PipeName  = "testpipe"
                    });
                case var t when t == typeof(ServiceOrchestratorContext):
                    return JsonConvert.SerializeObject(new ServiceOrchestratorConfiguration
                    {
                        OrchestratorName = "orchestrator-X",
                        ServiceOrchestratorBinding = new Binding {Host = "localhost"},
                        ExternalBindings = new List<ExternalBinding>(),
                    });
                default:
                    throw new InvalidOperationException($"Type {type.FullName} is not supported.");
            }
        }
    }
}
