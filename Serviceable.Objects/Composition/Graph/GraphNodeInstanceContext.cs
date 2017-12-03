namespace Serviceable.Objects.Composition.Graph
{
    using System.Collections.Generic;
    using System.Linq;
    using Commands.Node;

    public sealed class GraphNodeInstanceContext : Context<GraphNodeInstanceContext>
    {
        public readonly string Id;
        internal dynamic HostedContext { get; }
        internal AbstractContext HostedContextAsAbstractContext => HostedContext;
        internal readonly GraphContext GraphContext;
        internal readonly GraphNodeContext GraphNodeContext;

        public GraphNodeInstanceContext(AbstractContext hostedContext, GraphContext graphContext, GraphNodeContext graphNodeContext, string id)
        {
            HostedContext = hostedContext;
            GraphContext = graphContext;
            GraphNodeContext = graphNodeContext;
            Id = id;

            hostedContext.ContextEventPublished += OnContextEventPublished;
        }

        private IList<EventResult> OnContextEventPublished(IEvent eventPublished)
        {
            // Implement DFS on event propagation - because of the dependency in internal event generation
            return GraphNodeContext.Execute(new ProcessNodeEventLogic(eventPublished))
                .Where(x => x != null)
                .Select(x => new EventResult
                {
                    ResultObject = x.SingleContextExecutionResultWithInfo.ResultObject
                })
                .ToList();
        }
    }
}
