namespace Serviceable.Objects.Composition.Graph.Commands.NodeInstance
{
    using System.Threading;
    using Stages.Initialization;

    public sealed class InitializeNodeInstance : ICommand<GraphNodeInstanceContext, GraphNodeInstanceContext>
    {
        public GraphNodeInstanceContext Execute(GraphNodeInstanceContext context)
        {
            if (context.HostedContext is IInitializeStageFactory initialization)
            {
                if (initialization is IInitializationStageSynchronization initializationStageSynchronization)
                {
                    initializationStageSynchronization.ReaderWriterLockSlim = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
                }

                var command = initialization.GenerateInitializationCommand();

                if (command != null)
                {
                    context.HostedContext.Execute((dynamic) command);
                }
            }

            return context;
        }
    }
}