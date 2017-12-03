namespace Serviceable.Objects.Composition.Graph.Commands.Node
{
    using NodeInstance;

    public sealed class ProcessNodeEventLogic : ICommand<GraphNodeContext, GraphNodeContext>
    {
        private readonly IEvent eventPublished;

        public ProcessNodeEventLogic(IEvent eventPublished)
        {
            this.eventPublished = eventPublished;
        }

        public GraphNodeContext Execute(GraphNodeContext context)
        {
            // Algorithically, this will need to run in all hosted conexts (all graph node instances)
            foreach (var childNode in context.GraphContext.GetChildren(context.Id))
            {
                childNode.GraphNodeInstanceContext.Execute(new ProcessNodeInstanceEventLogic(eventPublished, context.GraphNodeInstanceContext));
            }

            return context;
        }
    }
}