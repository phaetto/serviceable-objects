namespace Serviceable.Objects.Remote.Composition.ServiceOrchestrator.Configuration
{
    using Objects.Composition.ServiceOrchestrator;

    public struct ServiceOrchestratorConfiguration
    {
        public string OrchestratorName { get; set; }
        public Binding ServiceOrchestratorBinding { get; set; }
        public ExternalBinding ExternalBinding { get; set; }
    }
}