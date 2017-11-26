namespace Serviceable.Objects.Composition.Graph
{
    using System.Collections.Generic;

    public interface IGraphFlowEventPushControl : IEvent
    {
        IEnumerable<EventResult> OverridePropagationLogic(GraphContext graphContext, string publishingNodeId, dynamic hostedContext);
    }
}
