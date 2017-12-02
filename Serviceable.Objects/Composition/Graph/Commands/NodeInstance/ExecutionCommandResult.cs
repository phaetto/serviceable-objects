namespace Serviceable.Objects.Composition.Graph.Commands.NodeInstance
{
    using System;
    using System.Collections.Generic;

    public class ExecutionCommandResult
    {
        public SingleContextExecutionResultWithInfo SingleContextExecutionResultWithInfo { get; set; }
        public List<IEvent> PublishedEvents { get; set; } = new List<IEvent>();
        public bool IsFaulted { get; set; }
        public bool IsIdle { get; set; }
        public Exception Exception { get; set; }
    }
}