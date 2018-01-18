namespace Serviceable.Objects.Remote.Composition.Configuration.Commands
{
    using Newtonsoft.Json;

    public sealed class ApplyConfiguration<TConfiguration, TContextType> : ICommand<ConfigurableContext<TConfiguration, TContextType>, ConfigurableContext<TConfiguration, TContextType>>
        where TConfiguration : struct
        where TContextType : Context<TContextType>
    {
        private readonly string configurationString;

        public ApplyConfiguration(string configurationString)
        {
            this.configurationString = configurationString;
        }

        public ConfigurableContext<TConfiguration, TContextType> Execute(ConfigurableContext<TConfiguration, TContextType> context)
        {
            if (!context.HasBeenConfigured)
            {
                context.SetConfiguration(JsonConvert.DeserializeObject<TConfiguration>(configurationString));
            }

            return context;
        }
    }
}