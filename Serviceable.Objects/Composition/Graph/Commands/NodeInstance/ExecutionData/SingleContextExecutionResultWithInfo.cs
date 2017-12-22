namespace Serviceable.Objects.Composition.Graph.Commands.NodeInstance.ExecutionData
{
    using System;

    public class SingleContextExecutionResultWithInfo
    {
        public string NodeId { get; set; }

        public Type ContextType { get; set; }

        public object ResultObject { get; set; }
    }
}