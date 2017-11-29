namespace Serviceable.Objects.Instrumentation.Server
{
    using System.Collections.Generic;
    using System.Linq;
    using CommonParameters;
    using Composition.Graph;
    using Composition.ServiceContainer;
    using Composition.ServiceOrchestrator;

    public sealed class InstrumentationServer : Context<InstrumentationServer>, IGraphFlowExecutionSink
    {
        private readonly IServiceOrchestrator serviceOrchestrator;
        private readonly IServiceContainer serviceContainer;
        internal CommonInstrumentationParameters CommonInstrumentationParameters;

        public InstrumentationServer(IServiceOrchestrator serviceOrchestrator = null, IServiceContainer serviceContainer = null)
        {
            this.serviceOrchestrator = serviceOrchestrator;
            this.serviceContainer = serviceContainer;
        }

        public dynamic CustomCommandExecute(GraphContext graphContext, string executingNodeId,
            dynamic commandApplied, Stack<EventResult> eventResults)
        {
            if (serviceOrchestrator != null) // TODO: we moved complexity to the depth of the graph - rethink this pls
            {
                // That means that the graph is on the service orchestrator level.
                return null;
            }
            else if (serviceContainer != null)
            {
                // That means that the graph is on the service container level.
                return serviceContainer.Services
                    .First(x => x.ServiceName == CommonInstrumentationParameters.ServiceName)
                    .GraphContext.GetNodeById(CommonInstrumentationParameters.NodeId)
                    .Execute(commandApplied)
                    .ResultObject;
            }
            else
            {
                // That means that instrumentaion is on a graph.
            }

            return null;
        }
    }
}
