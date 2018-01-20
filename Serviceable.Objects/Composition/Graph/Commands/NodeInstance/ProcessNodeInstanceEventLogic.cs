namespace Serviceable.Objects.Composition.Graph.Commands.NodeInstance
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using ExecutionData;
    using Microsoft.CSharp.RuntimeBinder;

    public sealed class ProcessNodeInstanceEventLogic : ICommand<GraphNodeInstanceContext, IEnumerable<ExecutionCommandResult>>
    {
        private readonly IEvent eventPublished;
        private readonly GraphNodeInstanceContext publishingGraphNodeInstanceContext;

        public ProcessNodeInstanceEventLogic(IEvent eventPublished, GraphNodeInstanceContext publishingGraphNodeInstanceContext)
        {
            this.eventPublished = eventPublished;
            this.publishingGraphNodeInstanceContext = publishingGraphNodeInstanceContext;
        }

        public IEnumerable<ExecutionCommandResult> Execute(GraphNodeInstanceContext context)
        {
            if (eventPublished is IGraphFlowEventPushControlEvent controlFlowEvent)
            {
                return controlFlowEvent.OverrideEventPropagationLogic(context.GraphContext, publishingGraphNodeInstanceContext.Id, publishingGraphNodeInstanceContext.HostedContext);
            }

            return new[] { EventPropagated(context) };
        }

        private ExecutionCommandResult EventPropagated(GraphNodeInstanceContext context)
        {
            var isEventTypeSupported =
                context.HostedContextAsAbstractContext.GetType().GetTypeInfo().ImplementedInterfaces.Any(x => InterfaceSupportsEventHandler(eventPublished, x));

            if (isEventTypeSupported)
            {
                object command;
                try
                {
                    command = context.HostedContext.GetCommandForEvent((dynamic)eventPublished);
                }
                catch (RuntimeBinderException runtimeBinderException)
                {
                    throw new InvalidOperationException($"Could not get command for event {eventPublished.GetType().AssemblyQualifiedName} on context {context.HostedContextAsAbstractContext.GetType().AssemblyQualifiedName}", runtimeBinderException);
                }

                return context.GraphNodeContext.ExecuteGraphCommand(command);
            }

            return null;
        }

        private static bool InterfaceSupportsEventHandler(IEvent eventPublished, Type interfaceType)
        {
            return interfaceType.GetGenericTypeDefinition() == typeof(IEventHandler<>) &&
                   interfaceType.GenericTypeArguments.Length == 1 &&
                   (
                       interfaceType.GenericTypeArguments[0] == eventPublished.GetType()
                       || interfaceType.GenericTypeArguments[0].GetTypeInfo().IsSubclassOf(eventPublished.GetType())
                       || GenericTypeSupportsDeclaredInterface(eventPublished, interfaceType)
                   );
        }

        private static bool GenericTypeSupportsDeclaredInterface(IEvent eventPublished, Type interfaceType)
        {
            return eventPublished.GetType().GetTypeInfo().ImplementedInterfaces.Any(y => y == interfaceType.GenericTypeArguments[0])
                   && interfaceType.GenericTypeArguments[0].GetTypeInfo().IsInterface;
        }
    }
}