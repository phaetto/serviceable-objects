namespace Serviceable.Objects.Remote
{
    using Serviceable.Objects.Remote.Serialization;

    public abstract class RemotableAction<TReceived, TContext> : Reproducible,
        IRemotableAction<TContext, TReceived>
        where TReceived : SerializableSpecification, new()
    {
        public abstract TReceived Execute(TContext context);
    }
}
