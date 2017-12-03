namespace Serviceable.Objects.Composition.Graph.Commands.Node
{
    using NodeInstance;

    public sealed class CheckNodePostGraphFlowPullControl : ICommand<GraphNodeContext, GraphNodeContext>
    {
        private readonly object command;

        public CheckNodePostGraphFlowPullControl(object command)
        {
            this.command = command;
        }

        public GraphNodeContext Execute(GraphNodeContext context)
        {
            // Algorithically, this will need to run in all hosted conexts (all graph node instances)
            foreach (var childNode in context.GraphContext.GetChildren(context.Id))
            {
                childNode.GraphNodeInstanceContext.Execute(new CheckNodeInstancePostGraphFlowPullControl(context.Id, context.HostedContext, command));
            }

            return context;
        }
    }
}