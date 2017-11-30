namespace Serviceable.Objects.IO.NamedPipes.Server.Commands
{
    using System.Threading.Tasks;

    public sealed class StartServer : ICommand<NamedPipeServerContext, NamedPipeServerContext>
    {
        public NamedPipeServerContext Execute(NamedPipeServerContext context)
        {
            context.ServerTask = Task.Run(() => context.RunServerAndBlock());
            return context;
        }
    }
}
