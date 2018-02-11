namespace Serviceable.Objects.Remote.Composition.ServiceOrchestrator.Commands.Data
{
    using System.Collections.Generic;
    using Dependencies;
    using Graph;

    public sealed class GraphAndDependencyInjectionData
    {
        public string TemplateName { get; set; }
        public string ServiceName { get; set; }
        public IEnumerable<GraphNode> GraphNodes { get; set; }
        public IEnumerable<GraphVertex> GraphVertices { get; set; }
        public IEnumerable<DependencyInjectionRegistration> Registrations { get; set; }
    }
}