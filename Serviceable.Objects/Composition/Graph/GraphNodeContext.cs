﻿namespace Serviceable.Objects.Composition.Graph
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.CSharp.RuntimeBinder;
    using Service;
    using Stages.Configuration;
    using Stages.Initialization;

    public sealed class GraphNodeContext
    {
        public readonly string Id;
        internal dynamic HostedContext { get; }
        private readonly GraphContext graphContext;
        private Stack<EventResult> localExecutionStack;
        private AbstractContext HostedContextAsAbstractContext => HostedContext;

        public GraphNodeContext(AbstractContext hostedContext, GraphContext graphContext, string id)
        {
            this.HostedContext = hostedContext;
            this.graphContext = graphContext;
            Id = id;

            hostedContext.CommandEventWithResultPublished += HostedContext_CommandEventWithResultPublished;
        }

        private IEnumerable<EventResult> HostedContext_CommandEventWithResultPublished(IEvent eventPublished)
        {
            if (eventPublished is IGraphFlowEventPushControl controlFlowEvent)
            {
                return controlFlowEvent.OverrideEventPropagationLogic(graphContext, Id, HostedContext);
            }

            return graphContext.GetChildren(Id)
                .Select(childNode => childNode.EventPropagated(eventPublished, localExecutionStack))
                .Where(eventResult => eventResult != null).ToList();
        }

        // TODO: break public methods to commands

        public void Configure(IConfigurationSource configurationSource)
        {
            if (HostedContext is IConfigurableStageFactory configurable && !configurable.HasBeenConfigured)
            {
                var command = configurable.GenerateConfigurationCommand(
                    graphContext.Container.Resolve<IService>(throwOnError: false),
                    graphContext,
                    this);

                HostedContext.Execute(command);
            }
        }

        public void Initialize()
        {
            if (HostedContext is IInitializeStageFactory initialization)
            {
                var command = initialization.GenerateInitializeCommand();
                HostedContext.Execute(command);
            }
        }

        public EventResult Execute(dynamic command, Stack<EventResult> resultExecutionStack = null)
        {
            try
            {
                localExecutionStack = resultExecutionStack;

                dynamic resultObject;
                try
                {
                    resultObject = HostedContext.Execute(command);
                }
                catch (RuntimeBinderException)
                {
                    if (HostedContextAsAbstractContext is IGraphFlowExecutionSink graphFlowExecutionSink)
                    {
                        resultObject = graphFlowExecutionSink.CustomCommandExecute(graphContext, Id, command, localExecutionStack);
                    }
                    else
                    {
                        throw;
                    }
                }

                foreach (var childNode in graphContext.GetChildren(Id))
                {
                    childNode.CheckPostGraphFlowPullControl(Id, HostedContext, command, localExecutionStack);
                }

                localExecutionStack = null;

                if (resultObject is AbstractContext)
                {
                    resultObject = null; // TODO: maybe replace the current hosted context if it is the same type? (respecting immutability)
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
                // TODO: Create a warning and error pool instead of throwing and catching?

                throw new NotSupportedException(
                    "This type of command is not supported by context (Tip: Only one implementation of ICommand<,> can be inferred automatically)",
                    ex);
            }
        }

        private void CheckPostGraphFlowPullControl(string id, dynamic parentContext, dynamic parentCommandApplied, Stack<EventResult> eventResults)
        {
            if (HostedContext is IPostGraphFlowPullControl hostedContextWithPullControl)
            {
                hostedContextWithPullControl.PullNodeExecutionInformation(graphContext, id, parentContext, parentCommandApplied, eventResults);
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
