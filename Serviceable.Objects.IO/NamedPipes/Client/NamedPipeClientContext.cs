namespace Serviceable.Objects.IO.NamedPipes.Client
{
    using System;
    using System.Threading.Tasks;
    using Commands;
    using Remote;
    using Remote.Proxying;
    using Remote.Serialization.Streaming;

    public sealed class NamedPipeClientContext : Context<NamedPipeClientContext>, ITypeSafeProxyContext
    {
        // TODO: use configuration, initialization
        internal readonly string NamedPipe;
        internal readonly int TimeoutInMilliseconds;
        internal readonly StreamSession StreamSession = new StreamSession();

        public NamedPipeClientContext(string namedPipe, int timeoutInMilliseconds)
        {
            NamedPipe = namedPipe;
            TimeoutInMilliseconds = timeoutInMilliseconds;
        }

        public IRemotableCarrier<TContext, TOtherContext, TReceived> CreateRemotableCarrier<TContext, TOtherContext, TReceived>()
            where TOtherContext : Context<TOtherContext> where TContext : ITypeSafeProxyContext
        {
            return new NamedPipeClientRemotableCarrier<TContext, TOtherContext, TReceived>();
        }

        public IReproducibleCarrier<TContext, TContext, TOtherContext, TReceived> CreateReproducibleCarrier<TContext, TOtherContext, TReceived>()
            where TOtherContext : Context<TOtherContext> where TContext : ITypeSafeProxyContext
        {
            return new NamedPipeClientReproducibleCarrier<TContext, TOtherContext, TReceived>();
        }

        public IReproducibleCarrier<TContext, Task<TContext> ,TOtherContext, TReceived> CreateAsyncReproducibleCarrier<TContext, TOtherContext, TReceived>()
            where TOtherContext : Context<TOtherContext> where TContext : ITypeSafeProxyContext
        {
            return new NamedPipeClientReproducibleCarrier<TContext, TOtherContext, TReceived>();
        }

        public object GenerateProxyCommandForGenericExecution(object commandToBeProxied)
        {
            if (commandToBeProxied is IReproducible reproducible)
            {
                return new Send(reproducible);
            }
            throw new NotSupportedException($"Command {commandToBeProxied.GetType().AssemblyQualifiedName} was not IReproducible");
        }

        public Send GenerateProxyCommandForGenericExecution(IReproducible reproducible)
        {
            return (Send) GenerateProxyCommandForGenericExecution((object) reproducible);
        }
    }
}
