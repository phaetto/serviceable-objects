namespace Serviceable.Objects.Composition.Graph.Commands.Node
{
    using System.Collections.Generic;
    using System.Linq;
    using NodeInstance;
    using Service;
    using ServiceOrchestrator;
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
            // Get configurations
            var nodeConfiguration =
                configurationSource?.GetConfigurationValueForKey(
                    service?.ServiceName ?? OrchestratorService,
                    context.Id,
                    context.ContextType.AssemblyQualifiedName);

            if ((service?.ExternalBinding?.AlgorithmBindings.Count ?? 0) == 0 || string.IsNullOrWhiteSpace(nodeConfiguration))
            {
                // Explicitly set
                var abstractContext = context.AbstractContext;

                if (abstractContext == null)
                {
                    abstractContext =
                        context.GraphContext.Container.CreateObject(context.ContextType) as AbstractContext;
                }

                var contextNodeInstance = new GraphNodeInstanceContext(abstractContext, context.GraphContext, context, context.Id);
                context.GraphNodeInstanceContextListPerAlgorithm.Add("default", new List<GraphNodeInstanceContext> { contextNodeInstance });

                if (!string.IsNullOrWhiteSpace(nodeConfiguration))
                {
                    contextNodeInstance.Execute(new ConfigureNodeInstance(nodeConfiguration));
                }
            }
            else
            {
                foreach (var algorithmBinding in service.ExternalBinding.AlgorithmBindings)
                {
                    var externalConfigurationSettings =
                        algorithmBinding.ScaleSetBindings.Select(x => ConfigureString(nodeConfiguration, x));

                    context.GraphNodeInstanceContextListPerAlgorithm.Add(algorithmBinding.AlgorithmTypeName, new List<GraphNodeInstanceContext>());

                    foreach (var externalConfigurationSetting in externalConfigurationSettings)
                    {
                        var abstractContext =
                            context.GraphContext.Container.CreateObject(context.ContextType) as AbstractContext;

                        var contextNodeInstance = new GraphNodeInstanceContext(abstractContext, context.GraphContext,
                            context, context.Id);
                        context.GraphNodeInstanceContextListPerAlgorithm[algorithmBinding.AlgorithmTypeName].Add(contextNodeInstance);
                        contextNodeInstance.Execute(new ConfigureNodeInstance(externalConfigurationSetting));
                    }
                }
            }
               
            return context;
        }

        private string ConfigureString(string setting, Binding binding)
        {
            return setting.Replace("$service.host", binding.Host)
                .Replace("$service.port", binding.Port.ToString())
                .Replace("$service.path", binding.Path);
        }
    }
}