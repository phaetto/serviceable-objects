namespace Serviceable.Objects.Composition.Graph
{
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
        internal readonly GraphNodeContext graphNodeContext;

        public GraphNodeInstanceContext(AbstractContext hostedContext, GraphContext graphContext, GraphNodeContext graphNodeContext, string id)
        {
            HostedContext = hostedContext;
            GraphContext = graphContext;
            this.graphNodeContext = graphNodeContext;
            Id = id;
        }

        // TODO: break public methods to commands

        public void Configure(IConfigurationSource configurationSource)
        {
            if (HostedContext is IConfigurableStageFactory configurable && !configurable.HasBeenConfigured)
            {
                var command = configurable.GenerateConfigurationCommand(
                    GraphContext.Container.Resolve<IService>(throwOnError: false),
                    GraphContext,
                    graphNodeContext);

                HostedContext.Execute(command); // TODO: immutability concerns
            }
        }

        public void Setup()
        {
            if (HostedContext is ISetupStageFactory graphSetup)
            {
                var command = graphSetup.GenerateSetupCommand(GraphContext, graphNodeContext);
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
    }
}
