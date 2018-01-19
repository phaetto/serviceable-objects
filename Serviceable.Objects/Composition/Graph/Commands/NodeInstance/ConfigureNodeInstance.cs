namespace Serviceable.Objects.Composition.Graph.Commands.NodeInstance
{
    using Stages.Configuration;

    public sealed class ConfigureNodeInstance : ICommand<GraphNodeInstanceContext, GraphNodeInstanceContext>
    {
        private readonly string serializedConfigurationString;

        public ConfigureNodeInstance(string serializedConfigurationString)
        {
            this.serializedConfigurationString = serializedConfigurationString;
        }

        public GraphNodeInstanceContext Execute(GraphNodeInstanceContext context)
        {
            if (context.HostedContext is IConfigurableStageFactory configurable && !configurable.HasBeenConfigured)
            {
                var command = configurable.GenerateConfigurationCommand(serializedConfigurationString);

                if (command != null)
                {
                    context.HostedContext.Execute((dynamic) command);
                }
            }

            return context;
        }
    }
}