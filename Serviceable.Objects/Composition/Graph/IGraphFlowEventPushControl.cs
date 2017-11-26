namespace Serviceable.Objects.Composition.Graph
{
    using System.Collections.Generic;

    public interface IGraphFlowEventPushControl : IEvent
    {
        IEnumerable<EventResult> OverridePropagationLogic(ContextGraph contextGraph, string publishingNodeId, dynamic hostedContext);
    }
}
