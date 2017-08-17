namespace Serviceable.Objects.Remote
{
    public abstract class ReproducibleAction<TContext, TReceived> : Reproducible, IReproducibleAction<TContext, TReceived>
    {
        public abstract TReceived Execute(TContext context);
    }
}