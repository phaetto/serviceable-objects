namespace Serviceable.Objects.Composition.ServiceOrchestrator
{
    using System.Collections.Generic;

    public class InBinding
    {
        public string ContextTypeName { get; set; }
        public IList<Binding> ScaleSetBindings { get; set; }
    }
}