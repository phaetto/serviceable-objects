namespace Serviceable.Objects.Composition.Graph.Commands.Node
{
    using System.Collections.Generic;
    using System.Linq;
    using NodeInstance;
    using NodeInstance.ExecutionData;

    public sealed class ProcessNodeEventLogic : ICommand<GraphNodeContext, IList<ExecutionCommandResult>>
    {
        private readonly IEvent eventPublished;

        public ProcessNodeEventLogic(IEvent eventPublished)
        {
            this.eventPublished = eventPublished;
        }

        public IList<ExecutionCommandResult> Execute(GraphNodeContext context)
        {
            return context.GraphContext.GetChildren(context.Id)
                .Select(x => x.GraphNodeInstanceContext.Execute(new ProcessNodeInstanceEventLogic(eventPublished, context.GraphNodeInstanceContext)))
                .SelectMany(x => x.Select(y => y)).ToArray();
        }
    }
}