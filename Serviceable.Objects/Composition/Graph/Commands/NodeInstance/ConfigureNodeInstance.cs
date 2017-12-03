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