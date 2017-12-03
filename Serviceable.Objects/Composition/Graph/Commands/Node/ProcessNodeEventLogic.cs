namespace Serviceable.Objects.Composition.Graph.Commands.Node
{
    using System.Collections.Generic;
    using System.Linq;
    using NodeInstance;
    using NodeInstance.ExecutionData;

    public sealed class ProcessNodeEventLogic : ICommand<GraphNodeContext, IEnumerable<ExecutionCommandResult>>
    {
        private readonly IEvent eventPublished;

        public ProcessNodeEventLogic(IEvent eventPublished)
        {
            this.eventPublished = eventPublished;
        }

        public IEnumerable<ExecutionCommandResult> Execute(GraphNodeContext context)
        {
            return context.GraphContext.GetChildren(context.Id)
                .Select(x => x.GraphNodeInstanceContext.Execute(new ProcessNodeInstanceEventLogic(eventPublished, context.GraphNodeInstanceContext)))
                .SelectMany(x => x.Select(y => y));
        }
    }
}