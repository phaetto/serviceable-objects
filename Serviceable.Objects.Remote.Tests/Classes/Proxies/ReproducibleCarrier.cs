namespace Serviceable.Objects.Remote.Tests.Classes.Proxies
{
    using System.Threading.Tasks;
    using Serviceable.Objects.Remote.Proxying;

    public sealed class ReproducibleCarrier<TContext, TOtherContext, TReceived> :
        IReproducibleCarrier<TContext, TContext, TOtherContext, TReceived>,
        IReproducibleCarrier<TContext, Task<TContext>, TOtherContext, TReceived>
        where TOtherContext : Context<TOtherContext>
        where TContext: IProxyContext
    {
        public TContext Execute(TContext context)
        {
            ((TOtherContext)context.WrappedContext).Execute(ReproducibleCommand);
            return context;
        }

        Task<TContext> ICommand<TContext, Task<TContext>>.Execute(TContext context)
        {
            ((TOtherContext)context.WrappedContext).Execute(ReproducibleCommand);
            return Task.FromResult(context);
        }

        public IReproducibleCommand<TOtherContext, TReceived> ReproducibleCommand { get; set; }
    }
}
