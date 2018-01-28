namespace Serviceable.Objects.Remote.Tests.Classes.Proxies.Command
{
    public sealed class ApplyGenericCommand : ICommand<TypeSafeProxyContext, object>
    {
        private readonly object commandToRun;

        public ApplyGenericCommand(object commandToRun)
        {
            this.commandToRun = commandToRun;
        }

        public object Execute(TypeSafeProxyContext context)
        {
            dynamic contextAsDynamic = context.WrappedContext;
            return contextAsDynamic.Execute((dynamic)commandToRun) as object;
        }
    }
}