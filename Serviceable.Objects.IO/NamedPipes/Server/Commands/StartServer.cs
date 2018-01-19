namespace Serviceable.Objects.IO.NamedPipes.Server.Commands
{
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class StartServer : ICommand<NamedPipeServerContext, NamedPipeServerContext>
    {
        public NamedPipeServerContext Execute(NamedPipeServerContext context)
        {
            context.CancellationTokenSource = new CancellationTokenSource();
            context.ServerTask = Task.Run(() => context.RunServerAndBlock(), context.CancellationTokenSource.Token);
            return context;
        }
    }
}
