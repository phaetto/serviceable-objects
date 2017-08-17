namespace Serviceable.Objects.Remote
{
    using Serviceable.Objects.Remote.Serialization;
    using Serviceable.Objects.Security;

    public abstract class Reproducible : IReproducible
    {
        public virtual ExecutableCommandSpecification GetInstanceSpec()
        {
            var securable = this as ISessionAuthorizableCommand;
            var apiAction = this as IApplicationAuthorizableCommand;

            return new ExecutableCommandSpecification
                   {
                       Type = GetType().FullName,
                       Session = securable?.Session,
                       ApiKey = apiAction?.ApiKey
                   };
        }
    }
}