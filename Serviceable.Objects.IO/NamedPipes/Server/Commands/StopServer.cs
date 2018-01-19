namespace Serviceable.Objects.IO.NamedPipes.Server.Commands
{
    public sealed class StopServer : ICommand<NamedPipeServerContext, NamedPipeServerContext>
    {
        public NamedPipeServerContext Execute(NamedPipeServerContext context)
        {
            context.CancellationTokenSource.Cancel();
            return context;
        }
    }
}
