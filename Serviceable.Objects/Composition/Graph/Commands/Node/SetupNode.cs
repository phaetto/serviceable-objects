namespace Serviceable.Objects.Composition.Graph.Commands.Node
{
    using NodeInstance;

    public sealed class SetupNode : ICommand<GraphNodeContext, GraphNodeContext>
    {
        public GraphNodeContext Execute(GraphNodeContext context)
        {
            context.GraphNodeInstanceContext.Execute(new SetupNodeInstance());
            return context;
        }
    }
}