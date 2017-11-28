namespace TestHttpCompositionConsoleApp.ConfigurationSources
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Serviceable.Objects.Composition.Graph.Stages.Configuration;
    using Serviceable.Objects.Composition.ServiceOrchestrator;
    using Serviceable.Objects.IO.NamedPipes.Server;
    using Serviceable.Objects.IO.NamedPipes.Server.Configuration;
    using Serviceable.Objects.Remote.Composition.ServiceOrchestrator;
    using Serviceable.Objects.Remote.Composition.ServiceOrchestrator.Configuration;

    public sealed class MemoryConfigurationSource : IConfigurationSource
    {
        public string GetConfigurationValueForKey(string serviceContainerName, string serviceName, string graphNodeId, string typeName)
        {
            switch (typeName)
            {
                case var s when s == typeof(NamedPipeServerContext).FullName:
                    return JsonConvert.SerializeObject(new NamedPipeServerConfiguration
                    {
                        PipeName  = "testpipe"
                    });
                case var s when s == typeof(ServiceOrchestratorContext).FullName:
                    return JsonConvert.SerializeObject(new ServiceOrchestratorConfiguration
                    {
                        OrchestratorName = "orchestrator-X",
                        ServiceOrchestratorBinding = new Binding {Host = "localhost"},
                        ExternalBindings = new List<ExternalBinding>(),
                    });
                default:
                    throw new InvalidOperationException($"Type {typeName} is not supported.");
            }
        }
    }
}
