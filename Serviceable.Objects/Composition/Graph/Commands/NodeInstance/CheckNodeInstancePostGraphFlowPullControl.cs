namespace Serviceable.Objects.Composition.Graph.Commands.NodeInstance
{
    public sealed class CheckNodeInstancePostGraphFlowPullControl : ICommand<GraphNodeInstanceContext, GraphNodeInstanceContext>
    {
        private readonly object parentContext;
        private readonly dynamic parentCommandApplied;
        private readonly string parentNodeId;

        public CheckNodeInstancePostGraphFlowPullControl(string parentNodeId, dynamic parentCommandApplied, object parentContext)
        {
            this.parentCommandApplied = parentCommandApplied;
            this.parentContext = parentContext;
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