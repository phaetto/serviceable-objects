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

            if (service?.Binding != null)
            {
                nodeConfiguration = ConfigureString(nodeConfiguration, service.Binding);
            }

            var contextBindings =
                service?.ExternalBindings?.FirstOrDefault(x =>
                    x.ContextTypeName == context.ContextType.AssemblyQualifiedName);

            if ((contextBindings?.AlgorithmBindings.Count ?? 0) == 0)
            {
                return new Dictionary<string, IEnumerable<string>>
                {
                    {DefaultAlgorithmicService, new[] {nodeConfiguration}}
                };
            }

            return contextBindings.AlgorithmBindings
                .ToDictionary(x => x.AlgorithmTypeName, y => y.ScaleSetBindings.Select(x => ConfigureString(nodeConfiguration, x)));
        }

        private string ConfigureString(string setting, Binding binding)
        {
            return setting.Replace("$external.host", binding.Host)
                .Replace("$external.port", binding.Port.ToString())
                .Replace("$external.path", binding.Path);
        }
    }
}