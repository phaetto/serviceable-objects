namespace Serviceable.Objects.Windows.ServiceCommands
{
    using Remote.Composition.Host;
    public sealed class Start : ICommand<ApplicationHost, ApplicationHost>
    {
        public ApplicationHost Execute(ApplicationHost context)
        {
            context.GraphContext.ConfigureSetupAndInitialize();
            return context;
        }
    }
}