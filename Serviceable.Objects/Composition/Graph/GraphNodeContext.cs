namespace Serviceable.Objects.Composition.Graph
{
    using Commands.Node;
    using Commands.NodeInstance;
    using Stages.Configuration;

    public sealed class GraphNodeContext : Context<GraphNodeContext>
    {
        public readonly string Id;
        internal dynamic HostedContext { get; }
        internal readonly GraphContext GraphContext;
        internal readonly GraphNodeInstanceContext GraphNodeInstanceContext;

        public GraphNodeContext(AbstractContext hostedContext, GraphContext graphContext, string id)
        {
            HostedContext = hostedContext;
            GraphContext = graphContext;
            Id = id;

            GraphNodeInstanceContext = new GraphNodeInstanceContext(hostedContext, graphContext, this, Id);
        }

        // TODO: break public methods to commands

        public void Configure(IConfigurationSource configurationSource)
        {
            GraphNodeInstanceContext.Configure(configurationSource);
        }

        public void Setup()
        {
            GraphNodeInstanceContext.Setup();
        }

        public void Initialize()
        {
            GraphNodeInstanceContext.Initialize();
        }

        public ExecutionCommandResult ExecuteGraphCommand(dynamic command)
        {
            // Events are propagated here and handled from GraphNodeInstanceContext::HostedContext_CommandEventWithResultPublished
            // We need this to invoke events from within the objects without any graph knowledge
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
