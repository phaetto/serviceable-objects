namespace Serviceable.Objects.Composition.Graph.Commands.NodeInstance
{
    using System;
    using Microsoft.CSharp.RuntimeBinder;

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
                dynamic resultObject;
                try
                {
                    resultObject = context.HostedContext.Execute(command);

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

                if (resultObject is AbstractContext)
                {
                    resultObject = null;
                    // TODO: maybe replace the current hosted context if it is the same type? (respecting immutability)
                }

                if (resultObject is Exception exception)
                {
                    executionCommandResult.SingleContextExecutionResultWithInfo =
                        new SingleContextExecutionResultWithInfo
                        {
                            NodeId = context.Id,
                            ContextType = context.HostedContextAsAbstractContext.GetType(),
                            ResultObject = null,
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
                            ResultObject = (object) resultObject,
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
    }
}