﻿namespace Serviceable.Objects.Composition.Graph.Commands.Node
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Service;
    using ServiceOrchestrator;
    using Stages.Configuration;

    public sealed class ExpandConfiguration : ICommand<GraphNodeContext, IDictionary<string, IEnumerable<string>>>, ISystemCommand
    {
        public const string DefaultAlgorithmicService = "default";
        public const string OrchestratorService = "orchestrator";
        public const string InConfigurablePrefix = "$in.";
        public const string OutConfigurablePrefix = "$out.";
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

            // TODO: add another generic In/Out without ContextTypeName after the specialized configuration that targets the type

            var contextInBinding =
                service?.InBindings?.FirstOrDefault(x =>
                    x.ContextTypeName == context.ContextType.AssemblyQualifiedName);

            var contextExternalBinding =
                service?.ExternalBindings?.FirstOrDefault(x =>
                    x.ContextTypeName == context.ContextType.AssemblyQualifiedName);

            var configurations = new List<string> {nodeConfiguration};

            if (contextInBinding != null)
            {
                configurations = contextInBinding.ScaleSetBindings.Select(x => ConfigureInString(nodeConfiguration, x))
                    .ToList();
            }

            if ((contextExternalBinding?.AlgorithmBindings?.Count ?? 0) == 0)
            {
                return CheckForUnconfigurableSettings(new Dictionary<string, IEnumerable<string>>
                {
                    {DefaultAlgorithmicService, configurations}
                });
            }

            // ReSharper disable once AssignNullToNotNullAttribute
            return CheckForUnconfigurableSettings(contextExternalBinding.AlgorithmBindings
                .ToDictionary(
                    x => x.AlgorithmTypeName, 
                    y => configurations.SelectMany(z => y.ScaleSetBindings.Select(x => ConfigureOutString(z, x)))
                ));
        }

        private Dictionary<string, IEnumerable<string>> CheckForUnconfigurableSettings(Dictionary<string, IEnumerable<string>> resultingConfiguration)
        {
            resultingConfiguration.ToList().ForEach(x => x.Value.ToList().ForEach(y =>
                {
                    if (y.Contains(InConfigurablePrefix) || y.Contains(OutConfigurablePrefix))
                    {
                        throw new InvalidOperationException($"Unconfigurable entity found at setting: {x.Key}/{y}");
                    }
                }));

            return resultingConfiguration;
        }

        private string ConfigureInString(string setting, Binding binding)
        {
            return binding.Aggregate(setting, (current, keyValuePair) => current.Replace($"{InConfigurablePrefix}{keyValuePair.Key}", keyValuePair.Value));
        }

        private string ConfigureOutString(string setting, Binding binding)
        {
            return binding.Aggregate(setting, (current, keyValuePair) => current.Replace($"{OutConfigurablePrefix}{keyValuePair.Key}", keyValuePair.Value));
        }
    }
}