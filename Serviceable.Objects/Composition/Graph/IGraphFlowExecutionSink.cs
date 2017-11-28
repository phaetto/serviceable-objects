namespace Serviceable.Objects.Composition.Graph
{
    using System.Collections.Generic;

    public interface IGraphFlowExecutionSink
    {
        dynamic CustomCommandExecute(GraphContext graphContext, string executingNodeId,
            dynamic parentCommandApplied, Stack<EventResult> eventResults);
    }
}