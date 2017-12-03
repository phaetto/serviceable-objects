namespace Serviceable.Objects.Composition.ServiceOrchestrator
{
    using System.Collections.Generic;

    public class ExternalBinding
    {
        public string ServiceName { get; set; }
        public IList<AlgorithmBinding> AlgorithmBindings { get; set; }
    }
}