namespace Serviceable.Objects.Remote.Proxying
{
    using System.Threading.Tasks;

    public interface IProxyContext
    {
        AbstractContext WrappedContext { get; }

        IRemotableCarrier<TContext, TOtherContext, TReceived> CreateRemotableCarrier<TContext, TOtherContext, TReceived>()
            where TOtherContext : Context<TOtherContext>
            where TContext : IProxyContext;

        IReproducibleCarrier<TContext, TContext, TOtherContext, TReceived> CreateReproducibleCarrier<TContext, TOtherContext, TReceived>()
            where TOtherContext : Context<TOtherContext>
            where TContext : IProxyContext;

        IReproducibleCarrier<TContext, Task<TContext>, TOtherContext, TReceived> CreateAsyncReproducibleCarrier<TContext, TOtherContext, TReceived>()
            where TOtherContext : Context<TOtherContext>
            where TContext : IProxyContext;
    }
}
