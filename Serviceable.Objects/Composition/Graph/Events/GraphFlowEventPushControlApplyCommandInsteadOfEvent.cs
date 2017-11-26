namespace Serviceable.Objects.Composition.Graph.Events
{
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

        public IEnumerable<EventResult> OverridePropagationLogic(ContextGraph contextGraph, string publishingNodeId, dynamic publishedHostedContext)
        {
            return contextGraph.GetChildren(publishingNodeId)
                .Select(ExecuteCommand)
                .Where(eventResult => eventResult != null).ToList();
        }

        private EventResult ExecuteCommand(ContextGraphNode childNode)
        {
            try
            {
                return childNode.Execute(commandToExecute);
            }
            catch (RuntimeBinderException)
            {
            }

            return null;
        }
    }
}
