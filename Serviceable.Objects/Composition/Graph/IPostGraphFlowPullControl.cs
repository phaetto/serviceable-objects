namespace Serviceable.Objects.Composition.Graph
{
    public interface IPostGraphFlowPullControl : IEvent
    {
        void GetAttachNodeCommandExecutionInformation(GraphContext graphContext, string executingNodeId, object parentContext, object parentCommandApplied);
    }
}
