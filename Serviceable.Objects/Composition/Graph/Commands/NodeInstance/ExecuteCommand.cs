namespace Serviceable.Objects.Composition.Graph.Commands.NodeInstance
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using ExecutionData;
    using Microsoft.CSharp.RuntimeBinder;
    using Proxying;

    public sealed class ExecuteCommand : ICommand<GraphNodeInstanceContext, ExecutionCommandResult>
    {
        private readonly dynamic command;
        private readonly ExecutionCommandResult executionCommandResult = new ExecutionCommandResult();

        public ExecuteCommand(dynamic command)
        {
            this.command = command;
        }

        public ExecutionCommandResult Execute(GraphNodeInstanceContext context)
        {
            try
            {
                object resultObject;
                try
                {
                    var commandToExecute = command;
                    if (context.HostedContextAsAbstractContext is IProxyFactoryContext proxyFactoryContext)
                    {
                        commandToExecute = proxyFactoryContext.GenerateProxyCommandForGenericExecution(command);
                    }

                    resultObject = context.HostedContext.Execute(commandToExecute);

                    // Event if the execution is proxied, the input command still carries the events
                    if (command is IEventProducer eventProducer)
                    {
                        executionCommandResult.PublishedEvents.AddRange(eventProducer.EventsProduced);
                    }
                }
                catch (RuntimeBinderException)
                {
                    if (context.HostedContextAsAbstractContext is IGraphFlowExecutionSink graphFlowExecutionSink)
                    {
                        resultObject = graphFlowExecutionSink.CustomCommandExecute(context.GraphContext, context.Id,
                            command);
                    }
                    else
                    {
                        throw;
                    }
                }

                if (resultObject is AbstractContext || IsTaskContext(resultObject))
                {
                    resultObject = null;
                    // TODO: maybe replace the current hosted context if it is the same type? (respecting immutability)
                }
                else if (resultObject is Task task)
                {
                    Task.WaitAll(task);

                    if (IsGenericTask(resultObject))
                    {
                        resultObject = (object) ((dynamic) task).Result;
                    }
                    else
                    {
                        resultObject = null;
                    }
                }

                if (resultObject is Exception exception)
                {
                    executionCommandResult.SingleContextExecutionResultWithInfo =
                        new SingleContextExecutionResultWithInfo
                        {
                            NodeId = context.Id,
                            ContextType = context.HostedContextAsAbstractContext.GetType(),
                            ResultObject = null
                        };
                    executionCommandResult.IsFaulted = true;
                    executionCommandResult.Exception = exception;
                }
                else
                {
                    executionCommandResult.SingleContextExecutionResultWithInfo =
                        new SingleContextExecutionResultWithInfo
                        {
                            NodeId = context.Id,
                            ContextType = context.HostedContextAsAbstractContext.GetType(),
                            ResultObject = resultObject
                        };
                }
            }
            catch (RuntimeBinderException exception)
            {
                executionCommandResult.IsIdle = true;
                executionCommandResult.Exception = exception;
            }
            catch (Exception exception)
            {
                executionCommandResult.IsFaulted = true;
                executionCommandResult.Exception = exception;
            }

            return executionCommandResult;
        }

        private static bool IsTaskContext(object returnedValue)
        {
            if (returnedValue == null)
            {
                return false;
            }

            var returnedValueType = returnedValue.GetType();
            return returnedValueType.GetTypeInfo().IsGenericType &&
                   returnedValueType.GetGenericTypeDefinition() == typeof(Task<>) &&
                   returnedValueType.GenericTypeArguments.Length == 1 &&
                   (
                       returnedValueType.GenericTypeArguments[0] == typeof(AbstractContext)
                       || returnedValueType.GenericTypeArguments[0].GetTypeInfo().IsSubclassOf(typeof(AbstractContext))
                   );
        }

        private static bool IsGenericTask(object returnedValue)
        {
            if (returnedValue == null)
            {
                return false;
            }

            var returnedValueType = returnedValue.GetType();
            return returnedValueType.GetTypeInfo().IsGenericType &&
                   returnedValueType.GetGenericTypeDefinition() == typeof(Task<>) &&
                   returnedValueType.GenericTypeArguments[0].GetTypeInfo().IsPublic;
        }
    }
}