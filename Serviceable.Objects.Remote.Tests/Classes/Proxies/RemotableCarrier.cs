namespace Serviceable.Objects.Remote.Tests.Classes.Proxies
{
    using Proxying;

    public sealed class RemotableCarrier<TContext, TOtherContext, TReceived> : IRemotableCarrier<TContext, TOtherContext, TReceived>
        where TOtherContext : Context<TOtherContext>
        where TContext : IProxyContext
    {
        public IRemotableCommand<TOtherContext, TReceived> RemotableCommand { get; set; }

        public TReceived Execute(TContext context)
        {
            return ((TOtherContext)((IProxyContentNeededData)context).WrappedContext).Execute(RemotableCommand); ;
        }
    }
}
