namespace Serviceable.Objects.Composition.Graph.Commands.NodeInstance
{
    public sealed class CheckNodeInstancePostGraphFlowPullControl : ICommand<GraphNodeInstanceContext, GraphNodeInstanceContext>
    {
        private readonly dynamic parentContext;
        private readonly dynamic parentCommandApplied;
        private readonly string parentNodeId;

        public CheckNodeInstancePostGraphFlowPullControl(string parentNodeId, dynamic parentContext, dynamic parentCommandApplied)
        {
            this.parentContext = parentContext;
            this.parentCommandApplied = parentCommandApplied;
            this.parentNodeId = parentNodeId;
        }

        public GraphNodeInstanceContext Execute(GraphNodeInstanceContext context)
        {
            if (context.HostedContext is IPostGraphFlowPullControl hostedContextWithPullControl)
            {
                hostedContextWithPullControl.GetAttachNodeCommandExecutionInformation(context.GraphContext, parentNodeId, parentContext, parentCommandApplied);
            }

            return context;
        }
    }
}