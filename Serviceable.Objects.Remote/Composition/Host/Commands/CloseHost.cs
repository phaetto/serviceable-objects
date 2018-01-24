namespace Serviceable.Objects.Remote.Composition.Host.Commands
{
    using Host;

    public class CloseHost : ReproducibleCommand<ApplicationHost, ApplicationHost>
    {
        public override ApplicationHost Execute(ApplicationHost context)
        {
            context.CancellationTokenSource.Cancel();
            return context;
        }
    }
}