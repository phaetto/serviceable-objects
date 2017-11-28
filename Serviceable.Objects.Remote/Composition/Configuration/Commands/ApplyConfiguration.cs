﻿namespace Serviceable.Objects.Remote.Composition.Configuration.Commands
{
    using System;
    using Newtonsoft.Json;
    using Objects.Composition.Graph;
    using Objects.Composition.Service;
    using Objects.Composition.ServiceContainer;

    public sealed class ApplyConfiguration<TConfiguration, TContextType> : ICommand<ConfigurableContext<TConfiguration, TContextType>, ConfigurableContext<TConfiguration, TContextType>>
        where TConfiguration : struct
        where TContextType : Context<TContextType>
    {
        private readonly IServiceContainer serviceContainer;
        private readonly IService service;
        private readonly GraphNodeContext graphNodeContext;

        public ApplyConfiguration(IServiceContainer serviceContainer, IService service, GraphNodeContext graphNodeContext)
        {
            this.serviceContainer = serviceContainer;
            this.service = service;
            this.graphNodeContext = graphNodeContext;
        }

        public ConfigurableContext<TConfiguration, TContextType> Execute(ConfigurableContext<TConfiguration, TContextType> context)
        {
            if (!context.HasBeenConfigured)
            {
                if (context.ConfigurationSource != null)
                {
                    var configurationString =
                        context.ConfigurationSource.GetConfigurationValueForKey(serviceContainer?.ContainerName, service?.ServiceName, graphNodeContext.Id, context.GetType().FullName);
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