namespace Serviceable.Objects.Remote.Proxying
{
    public interface IRemotableCarrier<in TContext, TOtherContext, TReceived> : ICommand<TContext, TReceived>
        where TOtherContext : Context<TOtherContext>
        where TContext : IProxyContext
    {
        IRemotableAction<TOtherContext, TReceived> RemotableAction { get; set; }
    }
}
