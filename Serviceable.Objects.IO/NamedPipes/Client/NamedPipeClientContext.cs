namespace Serviceable.Objects.IO.NamedPipes.Client
{
    using System.Threading.Tasks;
    using Commands;
    using Remote;
    using Remote.Proxying;
    using Remote.Serialization.Streaming;

    public sealed class NamedPipeClientContext : Context<NamedPipeClientContext>, IProxyContext
    {
        // TODO: use configuration, initialization ans sync
        internal readonly string NamedPipe;
        internal readonly int TimeoutInMilliseconds;
        internal readonly StreamSession StreamSession = new StreamSession();

        public NamedPipeClientContext(string namedPipe, int timeoutInMilliseconds)
        {
            NamedPipe = namedPipe;
            TimeoutInMilliseconds = timeoutInMilliseconds;
        }

        public IRemotableCarrier<TContext, TOtherContext, TReceived> CreateRemotableCarrier<TContext, TOtherContext, TReceived>()
            where TOtherContext : Context<TOtherContext> where TContext : IProxyContext
        {
            return new NamedPipeClientRemotableCarrier<TContext, TOtherContext, TReceived>();
        }

        public IReproducibleCarrier<TContext, TContext, TOtherContext, TReceived> CreateReproducibleCarrier<TContext, TOtherContext, TReceived>()
            where TOtherContext : Context<TOtherContext> where TContext : IProxyContext
        {
            return new NamedPipeClientReproducibleCarrier<TContext, TOtherContext, TReceived>();
        }

        public IReproducibleCarrier<TContext, Task<TContext> ,TOtherContext, TReceived> CreateAsyncReproducibleCarrier<TContext, TOtherContext, TReceived>()
            where TOtherContext : Context<TOtherContext> where TContext : IProxyContext
        {
            return new NamedPipeClientReproducibleCarrier<TContext, TOtherContext, TReceived>();
        }

        public object Execute(IReproducible command)
        {
            return Execute(new Send(command));
        }
    }
}
