namespace Serviceable.Objects.Instrumentation.Server
{
    using System;
    using System.Collections.Generic;
    using Commands;
    using CommonParameters;
    using Composition.Graph;
    using Composition.Graph.Stages.Setup;

    public sealed class InstrumentationServerContext : Context<InstrumentationServerContext>, IGraphFlowExecutionSink, ISetupStageFactory
    {
        internal CommonInstrumentationParameters CommonInstrumentationParameters;

        public dynamic CustomCommandExecute(GraphContext graphContext, string executingNodeId,
            dynamic commandApplied, Stack<EventResult> eventResults)
        {
            try
            {
                return graphContext.GetNodeById(CommonInstrumentationParameters.ContextId)
                    .Execute(commandApplied)
                    .ResultObject;
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
