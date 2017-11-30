namespace Serviceable.Objects.Remote
{
    using System;

    public abstract class RemotableCommand<TReceived, TContext> : Reproducible,
        IRemotableCommand<TContext, TReceived>,
        IReproducibleWithoutData
    {
        public Type ReturnType => typeof(TReceived);

        public abstract TReceived Execute(TContext context);
    }
}
