namespace Serviceable.Objects.Composition.Graph.Commands.Node
{
    using System;
    using System.Collections.Generic;
    using NodeInstance;
    using Service;
    using Stages.Configuration;

    public sealed class ConfigureNode : ICommand<GraphNodeContext, GraphNodeContext>
    {
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

            if (context.AbstractContext is IConfigurableStageFactory configurableStageFactory && configurableStageFactory.HasBeenConfigured
                || possibleConfigurations.Count == 0)
            {
                var abstractContext = context.AbstractContext ?? context.GraphContext.Container.CreateObject(context.ContextType) as AbstractContext;

                var contextNodeInstance = new GraphNodeInstanceContext(abstractContext, context.GraphContext, context, context.Id);
                context.GraphNodeInstanceContextListPerAlgorithm.Add(GetType(), new List<GraphNodeInstanceContext> { contextNodeInstance });

                return context;
            }

            foreach (var configuration in possibleConfigurations)
            {
                var algorithmicInstanceExecutionType = Type.GetType(configuration.Key, false);
                if (algorithmicInstanceExecutionType != null)
                {
                    var algorithmicInstanceExecution =
                        context.GraphContext.Container.CreateObject(algorithmicInstanceExecutionType) as IAlgorithmicInstanceExecution;

                    context.AlgorithmicInstanceExecutions.Add(algorithmicInstanceExecution);
                }

                algorithmicInstanceExecutionType = algorithmicInstanceExecutionType ?? GetType();

                context.GraphNodeInstanceContextListPerAlgorithm.Add(algorithmicInstanceExecutionType, new List<GraphNodeInstanceContext>());

                foreach (var externalConfigurationSetting in configuration.Value)
                {
                    var abstractContext =
                        context.GraphContext.Container.CreateObject(context.ContextType) as AbstractContext;

                    var contextNodeInstance = new GraphNodeInstanceContext(abstractContext, context.GraphContext,
                        context, context.Id);
                    context.GraphNodeInstanceContextListPerAlgorithm[algorithmicInstanceExecutionType].Add(contextNodeInstance);
                    contextNodeInstance.Execute(new ConfigureNodeInstance(externalConfigurationSetting));
                }
            }
               
            return context;
        }
    }
}