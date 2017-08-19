namespace Serviceable.Objects.Remote
{
    using Serviceable.Objects.Remote.Serialization;

    public abstract class RemotableCommand<TReceived, TContext> : Reproducible,
        IRemotableCommand<TContext, TReceived>
        where TReceived : SerializableSpecification, new()
    {
        public abstract TReceived Execute(TContext context);
    }
}
