namespace Serviceable.Objects.Remote.Composition.Host.Configuration
{
    using System;
    using Newtonsoft.Json;
    using Objects.Composition.Graph.Stages.Configuration;
    using ServiceOrchestrator;
    using ServiceOrchestrator.Configuration;

    public sealed class ServiceOrchestratorDefaultConfigurationSource : IConfigurationSource
    {
        private readonly ServiceOrchestratorConfiguration serviceOrchestratorConfiguration;

        public ServiceOrchestratorDefaultConfigurationSource(ServiceOrchestratorConfiguration serviceOrchestratorConfiguration)
        {
            this.serviceOrchestratorConfiguration = serviceOrchestratorConfiguration;
        }

        public string GetConfigurationValueForKey(string serviceName, string graphNodeId, string typeName)
        {
            if (typeName == typeof(ServiceOrchestratorContext).AssemblyQualifiedName)
            {
                return JsonConvert.SerializeObject(serviceOrchestratorConfiguration); // TODO: just send an object/TConfigurationType?
            }

            throw new InvalidOperationException("ServiceOrchestratorDefaultConfigurationSource only auto-configures ServiceOrchestrator");
        }
    }
}
