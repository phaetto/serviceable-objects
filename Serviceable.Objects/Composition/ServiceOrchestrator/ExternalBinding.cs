namespace Serviceable.Objects.Composition.ServiceOrchestrator
{
    using System.Collections.Generic;

    public class ExternalBinding
    {
        public string ContextTypeName { get; set; }
        public IList<AlgorithmBinding> AlgorithmBindings { get; set; }
    }
}