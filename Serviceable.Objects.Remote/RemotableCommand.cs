namespace Serviceable.Objects.Remote
{
    public abstract class RemotableCommand<TReceived, TContext> : Reproducible,
        IRemotableCommand<TContext, TReceived>,
        IReproducibleWithoutData
    {
        public abstract TReceived Execute(TContext context);
    }
}
