namespace Serviceable.Objects.Composition.Graph.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CSharp.RuntimeBinder;

    public sealed class GraphFlowEventPushControlApplyCommandInsteadOfEvent : IGraphFlowEventPushControl
    {
        private readonly dynamic commandToExecute;

        public GraphFlowEventPushControlApplyCommandInsteadOfEvent(dynamic commandToExecute)
        {
            this.commandToExecute = commandToExecute;
        }

        public IEnumerable<EventResult> OverrideEventPropagationLogic(GraphContext graphContext, string publishingNodeId, dynamic publishedHostedContext)
        {
            return graphContext.GetChildren(publishingNodeId)
                .Select(ExecuteCommand)
                .Where(eventResult => eventResult != null).ToList();
        }

        private EventResult ExecuteCommand(GraphNodeContext childNode)
        {
            try
            {
                return childNode.Execute(commandToExecute);
            }
            catch (NotSupportedException)
            {
            }

            return null;
        }
    }
}
