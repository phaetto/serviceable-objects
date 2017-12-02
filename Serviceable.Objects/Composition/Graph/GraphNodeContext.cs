namespace Serviceable.Objects.Composition.Graph
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Commands.NodeInstance;
    using Microsoft.CSharp.RuntimeBinder;
    using Stages.Configuration;

    public sealed class GraphNodeContext : Context<GraphNodeContext>
    {
        public readonly string Id;
        internal dynamic HostedContext { get; }
        private readonly GraphContext graphContext;
        private AbstractContext HostedContextAsAbstractContext => HostedContext;
        private readonly GraphNodeInstanceContext graphNodeInstanceContext;

        public GraphNodeContext(AbstractContext hostedContext, GraphContext graphContext, string id)
        {
            HostedContext = hostedContext;
            this.graphContext = graphContext;
            Id = id;

            graphNodeInstanceContext = new GraphNodeInstanceContext(hostedContext, graphContext, this, Id);

            hostedContext.CommandEventWithResultPublished += HostedContext_CommandEventWithResultPublished;
        }

        // TODO: break public methods to commands

        public void Configure(IConfigurationSource configurationSource)
        {
            graphNodeInstanceContext.Configure(configurationSource);
        }

        public void Setup()
        {
            graphNodeInstanceContext.Setup();
        }

        public void Initialize()
        {
            graphNodeInstanceContext.Initialize();
        }

        public ExecutionCommandResult Execute(dynamic command)
        {
            // Events are propagated here and handled from HostedContext_CommandEventWithResultPublished
            // We need this to invoke events from within the objects without any graph knowledge
            var contextExecutionResult = graphNodeInstanceContext.Execute(new ExecuteCommand(command));

            foreach (var childNode in graphContext.GetChildren(Id))
            {
                childNode.CheckPostGraphFlowPullControl(Id, HostedContext, command);
            }

            return contextExecutionResult;
        }

        private IEnumerable<EventResult> HostedContext_CommandEventWithResultPublished(IEvent eventPublished)
        {
            // Implement DFS - because of the dependency in internal event generation
            return graphContext.GetChildren(Id)
                .Select(x => ExecuteEventPropagationLogic(eventPublished, x))
                .SelectMany(x => x.Select(y => y))
                .Where(x => x != null)
                .Select(x => new EventResult
                {
                    ResultObject = x.SingleContextExecutionResultWithInfo.ResultObject
                });
        }

        private void CheckPostGraphFlowPullControl(string id, dynamic parentContext, dynamic parentCommandApplied)
        {
            if (HostedContext is IPostGraphFlowPullControl hostedContextWithPullControl)
            {
                hostedContextWithPullControl.GetAttachNodeCommandExecutionInformation(graphContext, id, parentContext, parentCommandApplied);
            }
        }

        private IEnumerable<ExecutionCommandResult> ExecuteEventPropagationLogic(IEvent eventPublished, GraphNodeContext graphNodeContext)
        {
            if (eventPublished is IGraphFlowEventPushControl controlFlowEvent)
            {
                return controlFlowEvent.OverrideEventPropagationLogic(graphContext, Id, HostedContext);
            }

            return new[] { graphNodeContext.EventPropagated(eventPublished) };
        }

        private ExecutionCommandResult EventPropagated(IEvent eventPublished)
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
                    throw new InvalidOperationException($"Could not get command for event {eventPublished.GetType().AssemblyQualifiedName} on context {HostedContextAsAbstractContext.GetType().AssemblyQualifiedName}", ex);
                }

                return Execute(command);
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
