namespace Serviceable.Objects.Composition.Graph
{
    using Commands.Node;
    using Commands.NodeInstance;
    using Commands.NodeInstance.ExecutionData;

    public sealed class GraphNodeContext : Context<GraphNodeContext>
    {
        public readonly string Id;
        internal dynamic HostedContext { get; }
        internal readonly GraphContext GraphContext;
        internal readonly GraphNodeInstanceContext GraphNodeInstanceContext;

        // TODO: define the algorithmic extensions
        // TODO: move the creation of the context on this level

        public GraphNodeContext(AbstractContext hostedContext, GraphContext graphContext, string id)
        {
            HostedContext = hostedContext;
            GraphContext = graphContext;
            Id = id;

            GraphNodeInstanceContext = new GraphNodeInstanceContext(hostedContext, graphContext, this, Id);
        }

        public ExecutionCommandResult ExecuteGraphCommand(dynamic command)
        {
            var contextExecutionResult = GraphNodeInstanceContext.Execute(new ExecuteCommand(command));

            foreach (var publishedEvent in contextExecutionResult.PublishedEvents)
            {
                Execute(new ProcessNodeEventLogic(publishedEvent));
            }

            Execute(new CheckNodePostGraphFlowPullControl(command));

            return contextExecutionResult;
        }
    }
}
