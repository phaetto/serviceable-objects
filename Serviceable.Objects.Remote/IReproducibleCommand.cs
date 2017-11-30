namespace Serviceable.Objects.Remote
{
    public interface IReproducibleCommand<in TContext, out TReceived> : ICommand<TContext, TReceived>, IReproducible
    {
    }
}
