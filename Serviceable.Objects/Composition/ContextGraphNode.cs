namespace Serviceable.Objects.Composition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.CSharp.RuntimeBinder;

    public sealed class ContextGraphNode
    {
        private readonly dynamic hostedContext;
        private readonly ContextGraph contextGraph;
        private Stack<EventResult> localExecutionStack;
        private dynamic HostedContext => hostedContext;
        private AbstractContext HostedContextAsAbstractContext => hostedContext;
        public readonly string Id;

        public ContextGraphNode(AbstractContext hostedContext, ContextGraph contextGraph, string id)
        {
            this.hostedContext = hostedContext;
            this.contextGraph = contextGraph;
            Id = id;

            hostedContext.CommandEventWithResultPublished += HostedContext_CommandEventWithResultPublished;
        }

        private IEnumerable<EventResult> HostedContext_CommandEventWithResultPublished(IEvent eventPublished)
        {
            if (eventPublished is IGraphFlowEventPushControl controlFlowEvent)
            {
                return controlFlowEvent.OverridePropagationLogic(contextGraph, Id, hostedContext);
            }

            return contextGraph.GetChildren(Id)
                .Select(childNode => childNode.EventPropagated(eventPublished, localExecutionStack))
                .Where(eventResult => eventResult != null).ToList();
        }

        public EventResult Execute(dynamic command, Stack<EventResult> resultExecutionStack = null)
        {
            try
            {
                localExecutionStack = resultExecutionStack;

                var resultObject = HostedContext.Execute(command);

                foreach (var childNode in contextGraph.GetChildren(Id))
                {
                    childNode.CheckPostGraphFlowPullControl(Id, hostedContext, command, localExecutionStack);
                }

                localExecutionStack = null;

                if (resultObject is AbstractContext)
                {
                    resultObject = null;
                }

                var eventResult = new EventResult
                {
                    NodeId = Id,
                    ContextType = HostedContextAsAbstractContext.GetType(),
                    ResultObject = (object) resultObject,
                };

                resultExecutionStack?.Push(eventResult);

                return eventResult;
            }
            catch (RuntimeBinderException ex)
            {
                throw new NotSupportedException(
                    "This type of command is not supported by context (Tip: Only one implementation of ICommand<,> can be inferred automatically)",
                    ex);
            }
        }

        private void CheckPostGraphFlowPullControl(string id, dynamic parentContext, dynamic parentCommandApplied, Stack<EventResult> eventResults)
        {
            if (hostedContext is IPostGraphFlowPullControl hostedContextWithPullControl)
            {
                hostedContextWithPullControl.PullNodeExecutionInformation(contextGraph, id, parentContext, parentCommandApplied, eventResults);
            }
        }

        private EventResult EventPropagated(IEvent eventPublished, Stack<EventResult> parentResultExecutionStack)
        {
            var isEventTypeSupported =
                HostedContextAsAbstractContext.GetType().GetTypeInfo().ImplementedInterfaces.Any(x => InterfaceSupportsEventHandler(eventPublished, x));

            if (isEventTypeSupported)
            {
                dynamic command;
                try
                {
                    command = HostedContext.GetCommandForEvent((dynamic)eventPublished);
                }
                catch (RuntimeBinderException ex)
                {
                    throw new InvalidOperationException($"Could not get command for event {eventPublished.GetType().FullName} on context {HostedContextAsAbstractContext.GetType().FullName}", ex);
                }

                return Execute(command, parentResultExecutionStack);
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
