namespace Serviceable.Objects.Composition.Graph.Commands.Node
{
    using System.Collections.Generic;
    using NodeInstance;
    using Service;
    using Stages.Configuration;

    public sealed class ConfigureNode : ICommand<GraphNodeContext, GraphNodeContext>
    {
        public const string OrchestratorService = "orchestrator";
        private readonly IService service;
        private readonly IConfigurationSource configurationSource;

        public ConfigureNode(IService service, IConfigurationSource configurationSource)
        {
            this.service = service;
            this.configurationSource = configurationSource;
        }

        public GraphNodeContext Execute(GraphNodeContext context)
        {
            var possibleConfigurations = context.Execute(new ExpandConfiguration(service, configurationSource));

            if (context.AbstractContext is IConfigurableStageFactory configurableStageFactory && configurableStageFactory.HasBeenConfigured)
            {
                var contextNodeInstance = new GraphNodeInstanceContext(context.AbstractContext, context.GraphContext, context, context.Id);
                context.GraphNodeInstanceContextListPerAlgorithm.Add(ExpandConfiguration.DefaultAlgorithmicService, new List<GraphNodeInstanceContext> { contextNodeInstance });

                return context;
            }

            if (possibleConfigurations.Count == 0)
            {
                var abstractContext = context.AbstractContext ?? context.GraphContext.Container.CreateObject(context.ContextType) as AbstractContext;

                var contextNodeInstance = new GraphNodeInstanceContext(abstractContext, context.GraphContext, context, context.Id);
                context.GraphNodeInstanceContextListPerAlgorithm.Add(ExpandConfiguration.DefaultAlgorithmicService, new List<GraphNodeInstanceContext> { contextNodeInstance });

                return context;
            }

            foreach (var configuration in possibleConfigurations)
            {
                context.GraphNodeInstanceContextListPerAlgorithm.Add(configuration.Key, new List<GraphNodeInstanceContext>());

                foreach (var externalConfigurationSetting in configuration.Value)
                {
                    var abstractContext =
                        context.GraphContext.Container.CreateObject(context.ContextType) as AbstractContext;

                    var contextNodeInstance = new GraphNodeInstanceContext(abstractContext, context.GraphContext,
                        context, context.Id);
                    context.GraphNodeInstanceContextListPerAlgorithm[configuration.Key].Add(contextNodeInstance);
                    contextNodeInstance.Execute(new ConfigureNodeInstance(externalConfigurationSetting));
                }
            }
               
            return context;
        }
    }
}