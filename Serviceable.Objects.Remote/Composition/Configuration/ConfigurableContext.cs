﻿namespace Serviceable.Objects.Remote.Composition.Configuration
{
    using System;
    using Commands;
    using Exceptions;
    using Objects.Composition.Graph.Stages.Configuration;

    public abstract class ConfigurableContext<TConfiguration, TContextType> : Context<TContextType>, IConfigurableStageFactory
        where TConfiguration : struct
        where TContextType : Context<TContextType>
    {
        internal readonly IConfigurationSource ConfigurationSource;
        public TConfiguration Configuration { get; private set; }
        public bool HasBeenConfigured { get; private set; }

        protected ConfigurableContext()
        {
        }

        protected ConfigurableContext(TConfiguration configuration)
        {
            HasBeenConfigured = true;
            Configuration = configuration;
        }

        protected ConfigurableContext(IConfigurationSource configurationSource)
        {
            ConfigurationSource = configurationSource;
        }

        public void SetConfiguration(TConfiguration state)
        {
            Check.Argument(HasBeenConfigured, nameof(state), $"The instance {GetType().AssemblyQualifiedName} has already been configured");

            Configuration = state;
            HasBeenConfigured = true;
        }

        protected override TReturnedContextType InvokeExecute<TReturnedContextType>(ICommand<TContextType, TReturnedContextType> action)
        {
            if (!HasBeenConfigured && !(action is ApplyConfiguration<TConfiguration, TContextType>))
            {
                throw new InvalidOperationException($"The instance {GetType().AssemblyQualifiedName} has already been configured yet.");
            }

            return base.InvokeExecute(action);
        }

        public dynamic GenerateConfigurationCommand(string serializedConfigurationString)
        {
            return new ApplyConfiguration<TConfiguration, TContextType>(serializedConfigurationString);
        }
    }
}
