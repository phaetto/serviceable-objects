namespace Serviceable.Objects.Composition.Graph.Commands.Node
{
    using System.Collections.Generic;
    using System.Linq;
    using Service;
    using ServiceOrchestrator;
    using Stages.Configuration;

    public sealed class ExpandConfiguration : ICommand<GraphNodeContext, IDictionary<string, IEnumerable<string>>>
    {
        public const string DefaultAlgorithmicService = "default";
        public const string OrchestratorService = "orchestrator";
        private readonly IService service;
        private readonly IConfigurationSource configurationSource;

        public ExpandConfiguration(IService service, IConfigurationSource configurationSource)
        {
            this.service = service;
            this.configurationSource = configurationSource;
        }

        public IDictionary<string, IEnumerable<string>> Execute(GraphNodeContext context)
        {
            // Get configuration string
            var nodeConfiguration =
                configurationSource?.GetConfigurationValueForKey(
                    service?.ServiceName ?? OrchestratorService,
                    context.Id,
                    context.ContextType.AssemblyQualifiedName);

            if (string.IsNullOrWhiteSpace(nodeConfiguration))
            {
                return new Dictionary<string, IEnumerable<string>>();
            }

            var contextInBindings =
                service?.InBindings?.FirstOrDefault(x =>
                    x.ContextTypeName == context.ContextType.AssemblyQualifiedName);

            var contextExternalBindings =
                service?.ExternalBindings?.FirstOrDefault(x =>
                    x.ContextTypeName == context.ContextType.AssemblyQualifiedName);

            var configurations = new List<string> {nodeConfiguration};

            if (contextInBindings != null)
            {
                configurations = contextInBindings.ScaleSetBindings.Select(x => ConfigureInString(nodeConfiguration, x))
                    .ToList();
            }

            if ((contextExternalBindings?.AlgorithmBindings?.Count ?? 0) == 0)
            {
                return new Dictionary<string, IEnumerable<string>>
                {
                    {DefaultAlgorithmicService, configurations}
                };
            }

            return contextExternalBindings.AlgorithmBindings
                .ToDictionary(
                    x => x.AlgorithmTypeName, 
                    y => configurations.SelectMany(z => y.ScaleSetBindings.Select(x => ConfigureOutString(z, x)))
                );
        }

        private string ConfigureInString(string setting, Binding binding)
        {
            return binding.Aggregate(setting, (current, keyValuePair) => current.Replace($"$in.{keyValuePair.Key}", keyValuePair.Value));
        }

        private string ConfigureOutString(string setting, Binding binding)
        {
            return binding.Aggregate(setting, (current, keyValuePair) => current.Replace($"$out.{keyValuePair.Key}", keyValuePair.Value));
        }
    }
}