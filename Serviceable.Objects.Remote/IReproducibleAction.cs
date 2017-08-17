namespace Serviceable.Objects.Remote
{
    public interface IReproducibleAction<in TContext, out TReceived> : ICommand<TContext, TReceived>, IReproducible
    {
    }
}
