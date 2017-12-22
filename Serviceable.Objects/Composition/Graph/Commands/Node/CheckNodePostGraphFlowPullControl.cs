namespace Serviceable.Objects.Composition.Graph.Commands.Node
{
    using System.Linq;
    using NodeInstance;

    public sealed class CheckNodePostGraphFlowPullControl : ICommand<GraphNodeContext, GraphNodeContext>
    {
        private readonly object parentContext;
        private readonly object command;

        public CheckNodePostGraphFlowPullControl(object command, object parentContext)
        {
            this.command = command;
            this.parentContext = parentContext;
        }

        public GraphNodeContext Execute(GraphNodeContext context)
        {
            // Algorithically, this will need to run in all hosted conexts (all graph node instances)
            foreach (var childNode in context.GraphContext.GetChildren(context.Id))
            {
                childNode.GraphNodeInstanceContextListPerAlgorithm
                    .SelectMany(x => x.Value)
                    .ToList().ForEach(
                        x => x.Execute(new CheckNodeInstancePostGraphFlowPullControl(context.Id, command, parentContext)));
            }

            return context;
        }
    }
}