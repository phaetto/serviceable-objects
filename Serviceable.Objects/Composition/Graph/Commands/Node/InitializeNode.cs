namespace Serviceable.Objects.Composition.Graph.Commands.Node
{
    using NodeInstance;

    public sealed class InitializeNode : ICommand<GraphNodeContext, GraphNodeContext>
    {
        public GraphNodeContext Execute(GraphNodeContext context)
        {
            context.GraphNodeInstanceContext.Execute(new InitializeNodeInstance());
            return context;
        }
    }
}