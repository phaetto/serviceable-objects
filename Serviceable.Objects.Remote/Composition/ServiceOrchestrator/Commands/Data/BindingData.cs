namespace Serviceable.Objects.Remote.Composition.ServiceOrchestrator.Commands.Data
{
    using System.Collections.Generic;
    using Objects.Composition.ServiceOrchestrator;

    public sealed class BindingData
    {
        public string TemplateName { get; set; }
        public string ServiceName { get; set; }
        public IEnumerable<InBinding> InBindings { get; set; }
        public IEnumerable<ExternalBinding> ExternalBindings { get; set; }
    }
}
