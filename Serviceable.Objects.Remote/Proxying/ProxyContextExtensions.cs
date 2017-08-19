namespace Serviceable.Objects.Remote.Proxying
{
    public static class ProxyContextExtensions
    {
        public static TResultType Execute<T, TResultType, TExecutionContext>(
            this TExecutionContext context,
            IRemotableCommand<T, TResultType> command)
            where T : Context<T>
            where TExecutionContext : Context<TExecutionContext>, IProxyContext
        {
            var remotableCarrier = context.CreateRemotableCarrier<TExecutionContext, T, TResultType>();
            remotableCarrier.RemotableCommand = command;
            return context.Execute(remotableCarrier);
        }

        public static TExecutionContext Execute<T, TResultType, TExecutionContext>(
            this TExecutionContext context,
            IReproducibleAction<T, TResultType> action)
            where TExecutionContext : Context<TExecutionContext>, IProxyContext
            where T : Context<T>
        {
            var reproducibleCarrier = context.CreateReproducibleCarrier<TExecutionContext, T, TResultType>();
            reproducibleCarrier.ReproducibleAction = action;
            return context.Execute(reproducibleCarrier);
        }
    }
}
