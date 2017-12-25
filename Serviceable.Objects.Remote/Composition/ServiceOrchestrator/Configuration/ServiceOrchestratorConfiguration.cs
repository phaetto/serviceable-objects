namespace Serviceable.Objects.Remote.Composition.ServiceOrchestrator.Configuration
{
    using System.Collections.Generic;
    using Objects.Composition.ServiceOrchestrator;

    public struct ServiceOrchestratorConfiguration
    {
        public string OrchestratorName { get; set; }
        public Dictionary<string, List<InBinding>> InBindingsPerService { get; set; }
        public Dictionary<string, List<ExternalBinding>> ExternalBindingsPerService { get; set; }
        public Dictionary<string, string> GraphTemplatesDictionary { get; set; }
    }
}