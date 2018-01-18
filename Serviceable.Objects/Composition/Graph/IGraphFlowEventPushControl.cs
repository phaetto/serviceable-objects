namespace Serviceable.Objects.Composition.Graph
{
    using System.Collections.Generic;
    using Commands.NodeInstance.ExecutionData;

    public interface IGraphFlowEventPushControl : IEvent
    {
        IEnumerable<ExecutionCommandResult> OverrideEventPropagationLogic(GraphContext graphContext, string publishingNodeId, object hostedContext);
    }
}
