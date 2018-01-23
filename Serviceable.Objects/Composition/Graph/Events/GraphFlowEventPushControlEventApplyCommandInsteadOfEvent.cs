﻿namespace Serviceable.Objects.Composition.Graph.Events
{
    using System.Collections.Generic;
    using System.Linq;
    using Commands.NodeInstance.ExecutionData;

    public sealed class GraphFlowEventPushControlEventApplyCommandInsteadOfEvent : IGraphFlowEventPushControlEvent
    {
        private readonly object commandToExecute;

        public GraphFlowEventPushControlEventApplyCommandInsteadOfEvent(object commandToExecute)
        {
            this.commandToExecute = commandToExecute;
        }

        public IEnumerable<ExecutionCommandResult> OverrideEventPropagationLogic(GraphContext graphContext, string publishingNodeId, object publishedHostedContext)
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