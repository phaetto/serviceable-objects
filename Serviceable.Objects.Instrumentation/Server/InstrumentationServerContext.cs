namespace Serviceable.Objects.Instrumentation.Server
{
    using System;
    using Commands;
    using CommonParameters;
    using Composition.Graph;
    using Composition.Graph.Commands.NodeInstance.ExecutionData;
    using Composition.Graph.Stages.Setup;

    public sealed class InstrumentationServerContext : Context<InstrumentationServerContext>, IGraphFlowExecutionSink, ISetupStageFactory
    {
        internal CommonInstrumentationParameters CommonInstrumentationParameters;

        public dynamic CustomCommandExecute(GraphContext graphContext, string executingNodeId,
            dynamic commandApplied)
        {
            try
            {
                var executionDataResult = (ExecutionCommandResult) graphContext.GetNodeById(CommonInstrumentationParameters.ContextId)
                    .ExecuteGraphCommand(commandApplied);

                return executionDataResult.SingleContextExecutionResultWithInfo.ResultObject;
            }
            catch (Exception exception)
            {
                return exception;
            }
        }

        public dynamic GenerateSetupCommand(GraphContext graphContext, GraphNodeContext graphNodeContext)
        {
            return new SetupServer(graphContext, graphNodeContext);
        }
    }
}
