namespace Serviceable.Objects.Composition.Graph.Commands.NodeInstance
{
    using System;
    using Stages.Initialization;

    public sealed class DeinitializeNodeInstance : ICommand<GraphNodeInstanceContext, GraphNodeInstanceContext>
    {
        public GraphNodeInstanceContext Execute(GraphNodeInstanceContext context)
        {
            if (context.HostedContext is IInitializeStageFactory initialization)
            {
                try
                {
                    if (initialization is IInitializationStageSynchronization initializationStageSynchronization)
                    {
                        initializationStageSynchronization.ReaderWriterLockSlim.EnterWriteLock();
                    }

                    var command = initialization.GenerateDeinitializationCommand();

                    if (command != null)
                    {
                        context.HostedContext.Execute((dynamic) command);
                    }
                }
                finally
                {
                    if (initialization is IInitializationStageSynchronization initializationStageSynchronization)
                    {
                        initializationStageSynchronization.ReaderWriterLockSlim.ExitWriteLock();
                        initializationStageSynchronization.ReaderWriterLockSlim.Dispose();
                    }
                }
            }

            return context;
        }
    }
}