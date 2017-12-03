namespace Serviceable.Objects.Composition.Graph.Commands.NodeInstance
{
    using Service;
    using Stages.Configuration;

    public sealed class ConfigureNodeInstance : ICommand<GraphNodeInstanceContext, GraphNodeInstanceContext>
    {
        public GraphNodeInstanceContext Execute(GraphNodeInstanceContext context)
        {
            if (context.HostedContext is IConfigurableStageFactory configurable && !configurable.HasBeenConfigured)
            {
                // TODO: Check if multiple configurations are available
                // TODO: The extend the instances of this node (not instance) to the number of the available configurations

                var command = configurable.GenerateConfigurationCommand(
                    context.GraphContext.Container.Resolve<IService>(throwOnError: false),
                    context.GraphContext,
                    context.GraphNodeContext);

                context.HostedContext.Execute(command); // TODO: immutability concerns / Break down the execute
            }

            return context;
        }
    }
}