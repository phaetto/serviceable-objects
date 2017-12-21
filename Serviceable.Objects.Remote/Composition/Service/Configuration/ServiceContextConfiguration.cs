namespace Serviceable.Objects.Remote.Composition.Service.Configuration
{
    using System.Collections.Generic;
    using Objects.Composition.ServiceOrchestrator;

    public struct ServiceContextConfiguration
    {
        public string OrchestratorName { get; set; }
        public string ServiceName { get; set; }
        public string TemplateName { get; set; }
        public Binding Binding { get; set; }
        public List<ExternalBinding> ExternalBindings { get; set; }
    }
}