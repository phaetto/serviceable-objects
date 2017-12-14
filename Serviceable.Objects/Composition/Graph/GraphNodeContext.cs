namespace Serviceable.Objects.Composition.Graph
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Commands.Node;
    using Commands.NodeInstance;
    using Commands.NodeInstance.ExecutionData;

    public sealed class GraphNodeContext : Context<GraphNodeContext>
    {
        public readonly string Id;
        public bool IsConfigured => GraphNodeInstanceContextListPerAlgorithm.Any();
        internal readonly GraphContext GraphContext;
        internal readonly Type ContextType;
        internal readonly Dictionary<string, List<GraphNodeInstanceContext>> GraphNodeInstanceContextListPerAlgorithm = new Dictionary<string, List<GraphNodeInstanceContext>>();
        internal readonly AbstractContext AbstractContext;

        // TODO: define the algorithmic extensions
        // TODO: move the creation of the context on this level

        public GraphNodeContext(Type contextType, GraphContext graphContext, string id)
        {
            ContextType = contextType;
            GraphContext = graphContext;
            Id = id;
        }

        public GraphNodeContext(AbstractContext abstractContext, GraphContext graphContext, string id)
        {
            ContextType = abstractContext.GetType();
            GraphContext = graphContext;
            Id = id;
            AbstractContext = abstractContext;
        }

        public ExecutionCommandResult ExecuteGraphCommand(dynamic command)
        {
            // TODO: algorithmic execution

            return GraphNodeInstanceContextListPerAlgorithm.First().Value.Select(x =>
            {
                var contextExecutionResult = x.Execute(new ExecuteCommand(command));

                foreach (var publishedEvent in contextExecutionResult.PublishedEvents)
                {
                    Execute(new ProcessNodeEventLogic(publishedEvent, x));
                }

                Execute(new CheckNodePostGraphFlowPullControl(command, x.HostedContext));

                return contextExecutionResult;
            }).FirstOrDefault();
        }
    }
}
