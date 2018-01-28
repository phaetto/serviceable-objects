namespace Serviceable.Objects.Remote.Proxying
{
    using System.Threading.Tasks;
    using Objects.Proxying;

    public interface ITypeSafeProxyContext : IProxyFactoryContext
    {
        IRemotableCarrier<TContext, TOtherContext, TReceived> CreateRemotableCarrier<TContext, TOtherContext, TReceived>()
            where TOtherContext : Context<TOtherContext>
            where TContext : ITypeSafeProxyContext;

        IReproducibleCarrier<TContext, TContext, TOtherContext, TReceived> CreateReproducibleCarrier<TContext, TOtherContext, TReceived>()
            where TOtherContext : Context<TOtherContext>
            where TContext : ITypeSafeProxyContext;

        IReproducibleCarrier<TContext, Task<TContext>, TOtherContext, TReceived> CreateAsyncReproducibleCarrier<TContext, TOtherContext, TReceived>()
            where TOtherContext : Context<TOtherContext>
            where TContext : ITypeSafeProxyContext;
    }
}
