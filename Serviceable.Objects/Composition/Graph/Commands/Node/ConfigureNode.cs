namespace Serviceable.Objects.Composition.Graph.Commands.Node
{
    using NodeInstance;

    public sealed class ConfigureNode : ICommand<GraphNodeContext, GraphNodeContext>
    {
        public GraphNodeContext Execute(GraphNodeContext context)
        {
            context.GraphNodeInstanceContext.Execute(new ConfigureNodeInstance());
            return context;
        }
    }
}