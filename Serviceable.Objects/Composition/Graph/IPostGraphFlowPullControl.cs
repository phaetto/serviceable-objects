namespace Serviceable.Objects.Composition.Graph
{
    public interface IPostGraphFlowPullControl : IEvent
    {
        void GetAttachNodeCommandExecutionInformation(GraphContext graphContext, string executingNodeId, dynamic parentContext, dynamic parentCommandApplied);
    }
}
