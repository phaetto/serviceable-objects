namespace Serviceable.Objects.Composition.Graph.Commands.Node
{
    using System.Collections.Generic;
    using System.Linq;
    using NodeInstance;
    using NodeInstance.ExecutionData;

    public sealed class ProcessNodeEventLogic : ICommand<GraphNodeContext, IList<ExecutionCommandResult>>, ISystemCommand
    {
        private readonly IEvent eventPublished;
        private readonly GraphNodeInstanceContext graphNodeInstanceContext;

        public ProcessNodeEventLogic(IEvent eventPublished, GraphNodeInstanceContext graphNodeInstanceContext)
        {
            this.eventPublished = eventPublished;
            this.graphNodeInstanceContext = graphNodeInstanceContext;
        }

        public IList<ExecutionCommandResult> Execute(GraphNodeContext context)
        {
            return context.GraphContext.GetChildren(context.Id)
                .SelectMany(x => x.GraphNodeInstanceContextListPerAlgorithm)
                .SelectMany(x => x.Value)
                .Select(x => x.Execute(new ProcessNodeInstanceEventLogic(eventPublished, graphNodeInstanceContext)))
                .SelectMany(x => x.Select(y => y)).ToArray();
        }
    }
}