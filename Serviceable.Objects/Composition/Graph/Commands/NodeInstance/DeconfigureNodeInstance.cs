namespace Serviceable.Objects.Composition.Graph.Commands.NodeInstance
{
    using System;
    using Stages.Configuration;

    public sealed class DeconfigureNodeInstance : ICommand<GraphNodeInstanceContext, GraphNodeInstanceContext>
    {
        public GraphNodeInstanceContext Execute(GraphNodeInstanceContext context)
        {
            if (context.HostedContext is IConfigurableStageFactory configurable)
            {
                var command = configurable.GenerateDeconfigurationCommand();

                if (command != null)
                {
                    context.HostedContext.Execute((dynamic) command);
                }
            }

            if (context.HostedContext is IDisposable disposable)
            {
                disposable.Dispose();
            }

            return context;
        }
    }
}