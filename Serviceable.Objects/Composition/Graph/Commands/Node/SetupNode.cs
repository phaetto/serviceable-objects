namespace Serviceable.Objects.Composition.Graph.Commands.Node
{
    using System.Linq;
    using NodeInstance;

    public sealed class SetupNode : ICommand<GraphNodeContext, GraphNodeContext>, ISystemCommand
    {
        public GraphNodeContext Execute(GraphNodeContext context)
        {
            context.Status = GraphNodeStatus.SetupStarted;

            context.GraphNodeInstanceContextListPerAlgorithm
                .SelectMany(x => x.Value)
                .ToList().ForEach(x => x.Execute(new SetupNodeInstance()));

            context.Status = GraphNodeStatus.SetupFinished;
            return context;
        }
    }
}