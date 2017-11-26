namespace Serviceable.Objects.Remote.Composition.Configuration
{
    using System;
    using Exceptions;
    using Newtonsoft.Json;
    using Objects.Composition;
    using Objects.Composition.Stages.Configuration;

    public abstract class ConfigurableContext<TConfiguration, TContextType> : Context<TContextType>, IConfigurable
        where TConfiguration : struct
        where TContextType : Context<TContextType>
    {
        protected readonly IConfigurationSource ConfigurationSource;
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
            this.ConfigurationSource = configurationSource;
        }

        public void SetConfiguration(TConfiguration state)
        {
            Check.Argument(HasBeenConfigured, nameof(state), "The instance has already been configured");

            Configuration = state;
            HasBeenConfigured = true;
        }

        protected override TReturnedContextType InvokeExecute<TReturnedContextType>(ICommand<TContextType, TReturnedContextType> action)
        {
            if (!HasBeenConfigured)
            {
                throw new InvalidOperationException("The instance has not been configured yet.");
            }

            return base.InvokeExecute(action);
        }

        public void Configure(ContextGraph contextGraph, ContextGraphNode contextGraphNode)
        {
            if (!HasBeenConfigured)
            {
                if (ConfigurationSource != null)
                {
                    var configurationString =
                        ConfigurationSource.GetConfigurationValueForKey(contextGraph, contextGraphNode, this.GetType());
                    SetConfiguration(JsonConvert.DeserializeObject<TConfiguration>(configurationString));
                    HasBeenConfigured = true;
                }
                else
                {
                    throw new InvalidOperationException("Configuration failed: No configuration source found.");
                }
            }
        }
    }
}
