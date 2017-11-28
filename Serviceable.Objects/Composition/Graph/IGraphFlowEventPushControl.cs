namespace Serviceable.Objects.Composition.Graph
{
    using System.Collections.Generic;

    public interface IGraphFlowEventPushControl : IEvent
    {
        IEnumerable<EventResult> OverrideEventPropagationLogic(GraphContext graphContext, string publishingNodeId, dynamic hostedContext);
    }
}
