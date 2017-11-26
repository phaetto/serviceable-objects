namespace Serviceable.Objects.Remote.Composition.Configuration.Commands
{
    using System;
    using Newtonsoft.Json;
    using Objects.Composition.Graph;
    using Objects.Composition.ServiceContainers;
    using Objects.Composition.Services;

    public sealed class ApplyConfiguration<TConfiguration, TContextType> : ICommand<ConfigurableContext<TConfiguration, TContextType>, ConfigurableContext<TConfiguration, TContextType>>
        where TConfiguration : struct
        where TContextType : Context<TContextType>
    {
        private readonly IServiceContainer serviceContainer;
        private readonly IService service;
        private readonly GraphContext graphContext;
        private readonly GraphNodeContext graphNodeContext;

        public ApplyConfiguration(IServiceContainer serviceContainer, IService service, GraphContext graphContext,
            GraphNodeContext graphNodeContext)
        {
            this.serviceContainer = serviceContainer;
            this.service = service;
            this.graphContext = graphContext;
            this.graphNodeContext = graphNodeContext;
        }

        public ConfigurableContext<TConfiguration, TContextType> Execute(ConfigurableContext<TConfiguration, TContextType> context)
        {
            if (!context.HasBeenConfigured)
            {
                if (context.ConfigurationSource != null)
                {
                    var configurationString =
                        context.ConfigurationSource.GetConfigurationValueForKey(serviceContainer, service, graphContext, graphNodeContext, context.GetType());
                    context.SetConfiguration(JsonConvert.DeserializeObject<TConfiguration>(configurationString));
                }
                else
                {
                    throw new InvalidOperationException("Configuration failed: No configuration source found.");
                }
            }

            return context;
        }
    }
}