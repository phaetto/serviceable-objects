﻿namespace Serviceable.Objects.Composition.Graph.Commands.NodeInstance
{
    using Stages.Setup;

    public sealed class SetupNodeInstance : ICommand<GraphNodeInstanceContext, GraphNodeInstanceContext>
    {
        public GraphNodeInstanceContext Execute(GraphNodeInstanceContext context)
        {
            if (context.HostedContext is ISetupStageFactory graphSetup)
            {
                var command = graphSetup.GenerateSetupCommand(context.GraphContext, context.GraphNodeContext);

                if (command != null)
                {
                    context.HostedContext.Execute((dynamic) command);
                }
            }

            return context;
        }
    }
}