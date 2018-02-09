namespace Serviceable.Objects.Composition.Graph.Events
{
    using Commands.NodeInstance.ExecutionData;

    public sealed class GraphFlowEventPushControlEventApplyCommandInsteadOfEvent : IGraphFlowEventPushControlEvent
    {
        private readonly object commandToExecute;

        public GraphFlowEventPushControlEventApplyCommandInsteadOfEvent(object commandToExecute)
        {
            this.commandToExecute = commandToExecute;
        }

        public ExecutionCommandResult OverrideEventPropagationLogic(GraphContext graphContext, string nodeId, object publishedHostedContext)
        {
            return graphContext.GetNodeById(nodeId).ExecuteGraphCommand(commandToExecute);
        }
    }
}
