namespace Serviceable.Objects.Composition
{
    using System.Collections.Generic;

    public interface IPostGraphFlowPullControl : IEvent
    {
        void PullNodeExecutionInformation(ContextGraph contextGraph, string executingNodeId, dynamic parentContext, dynamic parentCommandApplied, Stack<EventResult> eventResults);
    }
}
