namespace Serviceable.Objects.Instrumentation.Server
{
    using System.Collections.Generic;
    using CommonParameters;
    using Composition.Graph;

    public sealed class InstrumentationServer : Context<InstrumentationServer>, IGraphFlowExecutionSink
    {
        internal CommonInstrumentationParameters CommonInstrumentationParameters;

        public dynamic CustomCommandExecute(GraphContext graphContext, string executingNodeId,
            dynamic commandApplied, Stack<EventResult> eventResults)
        {
            return graphContext.GetNodeById(CommonInstrumentationParameters.ContextId)
                .Execute(commandApplied)
                .ResultObject;
        }
    }
}
