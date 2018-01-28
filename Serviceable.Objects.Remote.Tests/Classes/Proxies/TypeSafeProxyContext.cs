namespace Serviceable.Objects.Remote.Tests.Classes.Proxies
{
    using System.Threading.Tasks;
    using Command;
    using Proxying;

    public sealed class TypeSafeProxyContext : Context<TypeSafeProxyContext>, ITypeSafeProxyContext, IProxyContentNeededData
    {
        public AbstractContext WrappedContext { get; }

        public TypeSafeProxyContext(AbstractContext context)
        {
            WrappedContext = context;
        }

        public IRemotableCarrier<TContext, TOtherContext, TReceived> CreateRemotableCarrier<TContext, TOtherContext, TReceived>()
            where TOtherContext : Context<TOtherContext> where TContext : ITypeSafeProxyContext
        {
            return new RemotableCarrier<TContext, TOtherContext, TReceived>();
        }

        public IReproducibleCarrier<TContext, TContext, TOtherContext, TReceived> CreateReproducibleCarrier<TContext, TOtherContext, TReceived>()
            where TOtherContext : Context<TOtherContext> where TContext : ITypeSafeProxyContext
        {
            return new ReproducibleCarrier<TContext, TOtherContext, TReceived>();
        }

        public IReproducibleCarrier<TContext, Task<TContext> ,TOtherContext, TReceived> CreateAsyncReproducibleCarrier<TContext, TOtherContext, TReceived>()
            where TOtherContext : Context<TOtherContext> where TContext : ITypeSafeProxyContext
        {
            return new ReproducibleCarrier<TContext, TOtherContext, TReceived>();
        }

        public object GenerateProxyCommandForGenericExecution(object commandToBeProxied)
        {
            return new ApplyGenericCommand(commandToBeProxied);
        }
    }
}
