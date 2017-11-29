namespace Serviceable.Objects.Instrumentation.Server
{
    using System.Collections.Generic;
    using CommonParameters;
    using Composition.Graph;

    public sealed class InstrumentationServerContext : Context<InstrumentationServerContext>, IGraphFlowExecutionSink
    {
        internal CommonInstrumentationParameters CommonInstrumentationParameters;

        public dynamic CustomCommandExecute(GraphContext graphContext, string executingNodeId,
            dynamic commandApplied, Stack<EventResult> eventResults)
        {
            // TODO: error handling and reporting
            return graphContext.GetNodeById(CommonInstrumentationParameters.ContextId)
                .Execute(commandApplied)
                .ResultObject;
        }
    }
}
