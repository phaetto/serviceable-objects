namespace Serviceable.Objects.Composition.ServiceOrchestrator
{
    using System.Collections.Generic;

    public class AlgorithmBinding
    {
        public string AlgorithmName { get; set; }
        public IList<Binding> ScaleSetBindings { get; set; }
    }
}