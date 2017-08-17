namespace Serviceable.Objects.Remote.Proxying
{
    public interface IReproducibleCarrier<in TContext, out TResultContext, TOtherContext, TReceived>: ICommand<TContext, TResultContext>
        where TContext : IProxyContext
    {
        IReproducibleAction<TOtherContext, TReceived> ReproducibleAction { get; set; }
    }
}
