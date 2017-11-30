namespace Serviceable.Objects.Remote
{
    using System;

    public abstract class ReproducibleCommandWithSerializableData<TContext, TReceived, TDataType> : 
        Reproducible,
        IReproducibleCommand<TContext, TReceived>,
        IReproducibleWithKnownData<TDataType>
    {
        public TDataType Data { get; set; }
        public object DataAsObject => Data;
        public Type InitializationType => typeof(TDataType);

        protected ReproducibleCommandWithSerializableData(TDataType data)
        {
            Data = data;
        }

        public abstract TReceived Execute(TContext context);
    }
}