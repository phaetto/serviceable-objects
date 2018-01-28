namespace Serviceable.Objects.IO.NamedPipes.Client
{
    using System.Threading.Tasks;
    using Commands;
    using Remote;
    using Remote.Proxying;

    public sealed class NamedPipeClientReproducibleCarrier<TContext, TOtherContext, TReceived> :
        IReproducibleCarrier<TContext, TContext, TOtherContext, TReceived>,
        IReproducibleCarrier<TContext, Task<TContext>, TOtherContext, TReceived>
        where TOtherContext : Context<TOtherContext>
        where TContext: IProxyContext
    {
        public TContext Execute(TContext context)
        {
            (context as NamedPipeClientContext).Execute(new Send(ReproducibleCommand));
            return context;
        }

        Task<TContext> ICommand<TContext, Task<TContext>>.Execute(TContext context)
        {
            (context as NamedPipeClientContext).Execute(new Send(ReproducibleCommand));
            return Task.FromResult(context);
        }

        public IReproducibleCommand<TOtherContext, TReceived> ReproducibleCommand { get; set; }
    }
}
