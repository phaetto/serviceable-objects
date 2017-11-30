namespace Serviceable.Objects.Remote
{
    using System;

    public abstract class RemotableCommandWithSerializableData<TDataType, TReceived, TContext> :  // TODO: merge with the non-serializable version?
        Reproducible,
        IReproducibleWithKnownData<TDataType>,
        IRemotableCommand<TContext, TReceived>
    {
        public TDataType Data { get; set; }
        public object DataAsObject => Data;
        public Type ReturnType => typeof(TReceived);
        public Type InitializationType => typeof(TDataType);

        protected RemotableCommandWithSerializableData(TDataType data)
        {
            Data = data;
        }

        public abstract TReceived Execute(TContext context);
    }
}
