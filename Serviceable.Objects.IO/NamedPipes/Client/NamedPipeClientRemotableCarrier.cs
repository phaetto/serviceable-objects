namespace Serviceable.Objects.IO.NamedPipes.Client
{
    using Commands;
    using Remote;
    using Remote.Proxying;

    public sealed class NamedPipeClientRemotableCarrier<TContext, TOtherContext, TReceived> : IRemotableCarrier<TContext, TOtherContext, TReceived>
        where TOtherContext : Context<TOtherContext>
        where TContext : ITypeSafeProxyContext
    {
        public IRemotableCommand<TOtherContext, TReceived> RemotableCommand { get; set; }

        public TReceived Execute(TContext context)
        {
            return (context as NamedPipeClientContext).Execute(new Send(RemotableCommand)) is TReceived 
                ? (TReceived) (context as NamedPipeClientContext).Execute(new Send(RemotableCommand))
                : default(TReceived);
        }
    }
}
