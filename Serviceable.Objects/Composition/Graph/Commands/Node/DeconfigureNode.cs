namespace Serviceable.Objects.Composition.Graph.Commands.Node
{
    using NodeInstance;

    public sealed class DeconfigureNode : ICommand<GraphNodeContext, GraphNodeContext>, ISystemCommand
    {
        public GraphNodeContext Execute(GraphNodeContext context)
        {
            foreach (var algorithmAndInstancesListKeyValue in context.GraphNodeInstanceContextListPerAlgorithm)
            {
                foreach (var graphNodeInstanceContext in algorithmAndInstancesListKeyValue.Value)
                {
                    graphNodeInstanceContext.Execute(new DeconfigureNodeInstance());
                }
            }

            context.GraphNodeInstanceContextListPerAlgorithm.Clear();
            context.AlgorithmicInstanceExecutions.Clear();

            return context;
        }
    }
}