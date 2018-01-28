namespace Serviceable.Objects.Remote
{
    using System;

    public abstract class RemotableCommandWithData<TDataType, TReceived, TContext> : 
        Reproducible,
        IReproducibleWithKnownData<TDataType>,
        IRemotableCommand<TContext, TReceived>
    {
        public TDataType Data { get; set; }
        public object DataAsObject => Data;
        public Type ReturnType => typeof(TReceived);
        public Type InitializationType => typeof(TDataType);

        protected RemotableCommandWithData(TDataType data)
        {
            Data = data;
        }

        public abstract TReceived Execute(TContext context);
    }
}