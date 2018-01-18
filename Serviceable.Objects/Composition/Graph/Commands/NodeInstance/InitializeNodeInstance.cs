﻿namespace Serviceable.Objects.Composition.Graph.Commands.NodeInstance
{
    using Stages.Initialization;

    public sealed class InitializeNodeInstance : ICommand<GraphNodeInstanceContext, GraphNodeInstanceContext>
    {
        public GraphNodeInstanceContext Execute(GraphNodeInstanceContext context)
        {
            if (context.HostedContext is IInitializeStageFactory initialization)
            {
                var command = initialization.GenerateInitializeCommand();
                context.HostedContext.Execute((dynamic)command);
            }

            return context;
        }
    }
}