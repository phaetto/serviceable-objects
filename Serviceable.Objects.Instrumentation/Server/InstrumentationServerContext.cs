namespace Serviceable.Objects.Instrumentation.Server
{
    using System;
    using System.Linq;
    using Commands;
    using CommonParameters;
    using Objects.Composition.Graph;
    using Objects.Composition.Graph.Stages.Setup;
    using Remote.Composition.ServiceOrchestrator;

    public sealed class InstrumentationServerContext : Context<InstrumentationServerContext>, IGraphFlowExecutionSink, ISetupStageFactory
    {
        internal CommonInstrumentationParameters CommonInstrumentationParameters;
        internal GraphContext GraphContext;

        public object CustomCommandExecute(GraphContext graphContext, string executingNodeId,
            object commandApplied)
        {
            try
            {
                string nodeId;
                if (string.IsNullOrWhiteSpace(CommonInstrumentationParameters.ServiceName)
                    && string.IsNullOrWhiteSpace(CommonInstrumentationParameters.ContextId))
                {
                    // This needs to reach orchestrator
                    nodeId = graphContext.GetNodeIds<ServiceOrchestratorContext>().First();
                }
                else
                {
                    // We need to find the target context node to execute
                    nodeId = CommonInstrumentationParameters.ContextId;
                }

                var executionDataResult = graphContext.Execute(commandApplied, nodeId);

                return executionDataResult.SingleContextExecutionResultWithInfo?.ResultObject ?? executionDataResult.Exception;
            }
            catch (Exception exception)
            {
                return exception;
            }
        }

        public object GenerateSetupCommand(GraphContext graphContext, GraphNodeContext graphNodeContext)
        {
            GraphContext = graphContext;
            return new SetupServer(graphContext, graphNodeContext);
        }

        public object GenerateDismantleCommand(GraphContext graphContext, GraphNodeContext graphNodeContext)
        {
            GraphContext = null;
            return new DismantleServer(graphContext, graphNodeContext);
        }
    }
}
