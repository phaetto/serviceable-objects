namespace Serviceable.Objects.Composition.Graph.Events
{
    using System.Collections.Generic;
    using System.Linq;
    using Commands.NodeInstance.ExecutionData;

    public sealed class GraphFlowEventPushControlApplyCommandInsteadOfEvent : IGraphFlowEventPushControl
    {
        private readonly dynamic commandToExecute;

        public GraphFlowEventPushControlApplyCommandInsteadOfEvent(dynamic commandToExecute)
        {
            this.commandToExecute = commandToExecute;
        }

        public IEnumerable<ExecutionCommandResult> OverrideEventPropagationLogic(GraphContext graphContext, string publishingNodeId, dynamic publishedHostedContext)
        {
            return graphContext.GetChildren(publishingNodeId)
                .Select(ExecuteCommand);
        }

        private ExecutionCommandResult ExecuteCommand(GraphNodeContext childNode)
        {
            return childNode.ExecuteGraphCommand(commandToExecute);
        }
    }
}
