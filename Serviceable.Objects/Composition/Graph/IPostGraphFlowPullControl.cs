namespace Serviceable.Objects.Composition.Graph
{
    using System.Collections.Generic;

    public interface IPostGraphFlowPullControl : IEvent
    {
        void PullNodeExecutionInformation(GraphContext graphContext, string executingNodeId, dynamic parentContext, dynamic parentCommandApplied, Stack<EventResult> eventResults);
    }
}
