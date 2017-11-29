namespace Serviceable.Objects.Remote
{
    using Serviceable.Objects.Remote.Serialization;
    using Serviceable.Objects.Security;

    public abstract class ReproducibleCommandWithSerializableData<TContext, TReceived, TDataType> : 
        Reproducible,
        IReproducibleCommand<TContext, TReceived>,
        IReproducibleWithKnownData<TDataType>
    {
        public TDataType Data { get; set; }

        protected ReproducibleCommandWithSerializableData(TDataType data)
        {
            Data = data;
        }

        public abstract TReceived Execute(TContext context);

        public override ExecutableCommandSpecification GetInstanceSpec()
        {
            var secureAction = this as ISessionAuthorizableCommand;
            var apiAction = this as IApplicationAuthorizableCommand;

            return new ExecutableCommandSpecification
                   {
                       Data = Data,
                       DataType = Data?.GetType().FullName,
                       Type = GetType().FullName,
                       Session = secureAction?.Session,
                       ApiKey = apiAction?.ApiKey
                   };
        }
    }
}