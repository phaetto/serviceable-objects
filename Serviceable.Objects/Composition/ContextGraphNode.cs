namespace Serviceable.Objects.Composition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.CSharp.RuntimeBinder;

    internal sealed class ContextGraphNode
    {
        private readonly Stack<EventResult> rootExecutionStack;
        private readonly dynamic hostedContext;
        private readonly ContextGraph contextGraph;
        internal readonly ContextGraphNode RootNode;
        internal readonly Guid UniqueId = Guid.NewGuid();

        private Stack<EventResult> RootExecutionStack => RootNode != null ? RootNode.RootExecutionStack : rootExecutionStack;
        private dynamic HostedContext => hostedContext;
        private AbstractContext HostedContextAsAbstractContext => hostedContext;

        public ContextGraphNode(AbstractContext hostedContext, ContextGraph contextGraph, ContextGraphNode rootNode = null)
        {
            this.hostedContext = hostedContext;
            this.contextGraph = contextGraph;
            RootNode = rootNode;

            if (rootNode == null)
            {
                rootExecutionStack = new Stack<EventResult>();
            }

            hostedContext.CommandEventWithResultPublished += HostedContext_CommandEventWithResultPublished;
        }

        private IEnumerable<EventResult> HostedContext_CommandEventWithResultPublished(IEvent eventPublished)
        {
            var childNodes = contextGraph.AllNodes.Where(x => x.RootNode?.UniqueId == UniqueId);
            return childNodes.Select(childNode => childNode.EventPropagated(eventPublished)).Where(eventResult => eventResult != null).ToList();
        }

        public Stack<EventResult> Execute(dynamic command)
        {
            ExecuteInternal(command);
            return RootExecutionStack;
        }

        public EventResult ExecuteInternal(dynamic command)
        {
            try
            {
                var resultObject = HostedContext.Execute(command);

                var eventResult = new EventResult
                {
                    ContextType = HostedContextAsAbstractContext.GetType(),
                    ResultObject = (object) resultObject,
                };

                RootExecutionStack.Push(eventResult);

                return eventResult;
            }
            catch (RuntimeBinderException ex)
            {
                throw new NotSupportedException(
                    "This type of command is not supported by context (Tip: Only one implementation of ICommand<,> can be inferred automatically)",
                    ex);
            }
        }

        public EventResult EventPropagated(IEvent eventPublished)
        {
            var isEventTypeSupported =
                HostedContextAsAbstractContext.GetType().GetTypeInfo().GetInterfaces().Any(x => InterfaceSupportsEventHandler(eventPublished, x));

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

                return ExecuteInternal(command);
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
            return eventPublished.GetType().GetTypeInfo().GetInterfaces().Any(y => y == interfaceType.GenericTypeArguments[0])
                   && interfaceType.GenericTypeArguments[0].GetTypeInfo().IsInterface;
        }
    }
}
