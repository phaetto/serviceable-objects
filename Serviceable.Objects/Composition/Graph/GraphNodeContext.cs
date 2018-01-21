﻿namespace Serviceable.Objects.Composition.Graph
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Commands.Node;
    using Commands.NodeInstance;
    using Commands.NodeInstance.ExecutionData;

    public sealed class GraphNodeContext : Context<GraphNodeContext>
    {
        public bool IsConfigured => GraphNodeInstanceContextListPerAlgorithm.Any();
        public bool IsWorking => WorkingReferenceCount > 0;
        public readonly string Id;
        internal readonly GraphContext GraphContext;
        internal readonly Type ContextType;
        internal readonly List<IAlgorithmicInstanceExecution> AlgorithmicInstanceExecutions = new List<IAlgorithmicInstanceExecution>();
        internal readonly Dictionary<Type, List<GraphNodeInstanceContext>> GraphNodeInstanceContextListPerAlgorithm = new Dictionary<Type, List<GraphNodeInstanceContext>>();
        internal readonly AbstractContext AbstractContext;
        internal int WorkingReferenceCount;

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

        internal ExecutionCommandResult ExecuteGraphCommand(object command)
        {
            try
            {
                Interlocked.Increment(ref WorkingReferenceCount);

                if (!AlgorithmicInstanceExecutions.Any())
                {
                    // Default execution - find first and run it
                    return GraphNodeInstanceContextListPerAlgorithm.First().Value.Select(x => ExecutionLogicOnNodeInstance(command, x)).FirstOrDefault();
                }

                // The first algorithm runs and the others are inspecting
                IList<ExecutionCommandResult> firstExecutionCommandResults = null;
                foreach (var algorithmicInstanceExecution in AlgorithmicInstanceExecutions)
                {
                    var algorithmType = algorithmicInstanceExecution.GetType();

                    if (firstExecutionCommandResults == null)
                    {
                        // The first execution is the master algorithm
                        var nodeInstancesToBeExecuted =
                            algorithmicInstanceExecution.FindGraphNodeInstanceContextsToBeExecuted(
                                GraphNodeInstanceContextListPerAlgorithm[algorithmType]
                            );

                        firstExecutionCommandResults = nodeInstancesToBeExecuted.Select(x => ExecutionLogicOnNodeInstance(command, x)).ToList();
                    }
                    else
                    {
                        // Select nodes that would need to run
                        var nodeInstancesToBeExecuted =
                            algorithmicInstanceExecution.ContinueExecutionGraphNodeInstanceContextsToBeExecuted(
                                firstExecutionCommandResults,
                                GraphNodeInstanceContextListPerAlgorithm[algorithmType]
                            );

                        // Execute them
                        var newExecutionCommandResults = nodeInstancesToBeExecuted.Select(x => ExecutionLogicOnNodeInstance(command, x)).ToList();

                        // Possibly manipulate the returned results
                        firstExecutionCommandResults =
                            algorithmicInstanceExecution.FilterExecutionResults(firstExecutionCommandResults,
                                newExecutionCommandResults);
                    }
                }

                // ReSharper disable once AssignNullToNotNullAttribute
                return firstExecutionCommandResults.FirstOrDefault(x => x != null);
            }
            finally
            {
                Interlocked.Decrement(ref WorkingReferenceCount);
            }
        }

        private ExecutionCommandResult ExecutionLogicOnNodeInstance(object command, GraphNodeInstanceContext graphNodeInstanceContext)
        {
            var contextExecutionResult = graphNodeInstanceContext.Execute(new ExecuteCommand(command));

            foreach (var publishedEvent in contextExecutionResult.PublishedEvents)
            {
                Execute(new ProcessNodeEventLogic(publishedEvent, graphNodeInstanceContext));
            }

            Execute(new CheckNodePostGraphFlowPullControl(command, graphNodeInstanceContext.HostedContextAsAbstractContext));

            return contextExecutionResult;
        }
    }
}
