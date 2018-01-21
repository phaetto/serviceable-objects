namespace Serviceable.Objects.Composition.Graph
{
    using System.Collections.Generic;
    using Commands.NodeInstance.ExecutionData;

    public interface IGraphFlowEventPushControlEvent : IEvent
    {
        IEnumerable<ExecutionCommandResult> OverrideEventPropagationLogic(GraphContext graphContext, string publishingNodeId, object hostedContext);
    }
}
