namespace Serviceable.Objects.Remote
{
    using Serviceable.Objects.Remote.Serialization;
    using Serviceable.Objects.Security;

    public abstract class RemotableActionWithSerializableData<TDataType, TReceived, TContext> : Reproducible,
        IRemotableAction<TContext, TReceived>
    {
        public TDataType Data { get; set; }

        protected RemotableActionWithSerializableData(TDataType data)
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
