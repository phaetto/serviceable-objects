namespace Serviceable.Objects.Remote.Composition.Configuration.Commands
{
    public sealed class Deconfigure<TConfiguration, TContextType> : ICommand<ConfigurableContext<TConfiguration, TContextType>, ConfigurableContext<TConfiguration, TContextType>>
        where TConfiguration : struct
        where TContextType : Context<TContextType>
    {
        public ConfigurableContext<TConfiguration, TContextType> Execute(ConfigurableContext<TConfiguration, TContextType> context)
        {
            context.ClearConfiguration();
            return context;
        }
    }
}