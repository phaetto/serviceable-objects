namespace Serviceable.Objects.Remote
{
    public abstract class ReproducibleCommand<TContext, TReceived> : Reproducible,
        IReproducibleCommand<TContext, TReceived>,
        IReproducibleWithoutData
    {
        public abstract TReceived Execute(TContext context);
    }
}