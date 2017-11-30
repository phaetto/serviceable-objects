namespace Serviceable.Objects.Remote.Composition.ServiceOrchestrator.Configuration
{
    using System.Collections.Generic;
    using Objects.Composition.ServiceOrchestrator;

    public struct ServiceOrchestratorConfiguration
    {
        public string OrchestratorName { get; set; }
        public Binding ServiceOrchestratorBinding { get; set; }
        public IList<ExternalBinding> ExternalBindings { get; set; }
    }
}