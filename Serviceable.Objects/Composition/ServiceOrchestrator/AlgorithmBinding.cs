namespace Serviceable.Objects.Composition.ServiceOrchestrator
{
    using System.Collections.Generic;

    public class AlgorithmBinding
    {
        public string AlgorithmTypeName { get; set; }
        public IList<Binding> ScaleSetBindings { get; set; }
    }
}