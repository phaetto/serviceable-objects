namespace Serviceable.Objects.Remote.Serialization
{
    using System;
    using Serviceable.Objects.Remote.Dependencies;
    using Serviceable.Objects.Security;

    [Serializable]
    public class ExecutableCommandSpecification : SerializableSpecification
    {
        public string Type;

        public string DataType;

        public object Data;

        public string Session;

        public string ApiKey;

        public override int DataStructureVersionNumber => 1;

        public T CreateFromSpec<T>()
        {
            if (string.IsNullOrWhiteSpace(Type))
            {
                throw new NullReferenceException("Type cannot be null nor a whitespace");
            }

            object generatedObject = null;

            if (Data == null)
            {
                generatedObject = Types.CreateObjectWithParameters(Type);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(DataType))
                {
                    throw new NullReferenceException("DataType cannot be null nor a whitespace when data exist.");
                }

                generatedObject = Types.CreateObjectWithParameters(Type, Data);
            }

            if (generatedObject == null)
            {
                return default(T);
            }

            var securable = generatedObject as ISessionAuthorizableCommand;
            var apiAction = generatedObject as IApplicationAuthorizableCommand;

            if (securable != null)
            {
                securable.Session = Session;
            }

            if (apiAction != null)
            {
                apiAction.ApiKey = ApiKey;
            }

            return (T)generatedObject;
        }

        public object CreateFromSpec()
        {
            return CreateFromSpec<object>();
        }
    }
}
