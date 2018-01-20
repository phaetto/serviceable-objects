namespace Serviceable.Objects.Composition.Graph.Commands.Node
{
    using System.Linq;
    using NodeInstance;

    public sealed class DismantleNode : ICommand<GraphNodeContext, GraphNodeContext>, ISystemCommand
    {
        public GraphNodeContext Execute(GraphNodeContext context)
        {
            context.GraphNodeInstanceContextListPerAlgorithm
                .SelectMany(x => x.Value)
                .ToList().ForEach(x => x.Execute(new DismantleNodeInstance()));

            return context;
        }
    }
}