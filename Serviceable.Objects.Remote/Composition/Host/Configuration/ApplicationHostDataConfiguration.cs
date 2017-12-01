namespace Serviceable.Objects.Remote.Composition.Host.Configuration
{
    using Dependencies;
    using Graph;
    using Service.Configuration;
    using ServiceOrchestrator.Configuration;

    public sealed class ApplicationHostDataConfiguration
    {
        public DependencyInjectionRegistrationTemplate DependencyInjectionRegistrationTemplate { get; set; }
        public GraphTemplate ServiceGraphTemplate { get; set; }
        public GraphTemplate OrchestratorOverrideTemplate { get; set; }
        public ServiceContextConfiguration ServiceContextConfiguration { get; set; }
        public ServiceOrchestratorConfiguration ServiceOrchestratorConfiguration { get; set; }
    }
}
