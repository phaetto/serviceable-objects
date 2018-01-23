namespace Serviceable.Objects.Windows.ServiceCommands
{
    using Remote.Composition.Host;

    public sealed class Stop : ICommand<ApplicationHost, ApplicationHost>
    {
        public ApplicationHost Execute(ApplicationHost context)
        {
            context.CancellationTokenSource.Cancel();
            context.GraphContext.UninitializeDismantleAndDeconfigure();
            return context;
        }
    }
}