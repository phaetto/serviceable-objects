namespace Serviceable.Objects.Composition.Graph
{
    using System.Collections.Generic;
    using System.Linq;
    using Commands.NodeInstance;
    using Service;
    using Stages.Configuration;
    using Stages.Initialization;
    using Stages.Setup;

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

        // TODO: break public methods to commands

        public void Configure(IConfigurationSource configurationSource)
        {
            if (HostedContext is IConfigurableStageFactory configurable && !configurable.HasBeenConfigured)
            {
                var command = configurable.GenerateConfigurationCommand(
                    GraphContext.Container.Resolve<IService>(throwOnError: false),
                    GraphContext,
                    GraphNodeContext);

                HostedContext.Execute(command); // TODO: immutability concerns
            }
        }

        public void Setup()
        {
            if (HostedContext is ISetupStageFactory graphSetup)
            {
                var command = graphSetup.GenerateSetupCommand(GraphContext, GraphNodeContext);
                HostedContext.Execute(command);
            }
        }

        public void Initialize()
        {
            if (HostedContext is IInitializeStageFactory initialization)
            {
                var command = initialization.GenerateInitializeCommand();
                HostedContext.Execute(command);
            }
        }

        private IList<EventResult> OnContextEventPublished(IEvent eventPublished)
        {
            // Implement DFS on event propagation - because of the dependency in internal event generation
            return GraphContext.GetChildren(Id)
                .Select(x => x.GraphNodeInstanceContext.Execute(new ProcessNodeInstanceEventLogic(eventPublished, this)))
                .SelectMany(x => x.Select(y => y))
                .Where(x => x != null)
                .Select(x => new EventResult
                {
                    ResultObject = x.SingleContextExecutionResultWithInfo.ResultObject
                })
                .ToList();
        }
    }
}
