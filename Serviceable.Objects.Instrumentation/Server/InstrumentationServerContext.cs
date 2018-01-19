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
                GraphNodeContext graphNodeContextForExecution;
                if (string.IsNullOrWhiteSpace(CommonInstrumentationParameters.ServiceName)
                    && string.IsNullOrWhiteSpace(CommonInstrumentationParameters.ContextId))
                {
                    // This needs to reach orchestrator
                    graphNodeContextForExecution = graphContext.GetNodeById(graphContext.GetNodeIds<ServiceOrchestratorContext>().First());
                }
                else
                {
                    // We need to find the target context node to execute
                    graphNodeContextForExecution = graphContext.GetNodeById(CommonInstrumentationParameters.ContextId);
                }

                var executionDataResult = graphNodeContextForExecution.ExecuteGraphCommand(commandApplied);

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
