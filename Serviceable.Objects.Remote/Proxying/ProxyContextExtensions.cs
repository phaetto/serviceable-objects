namespace Serviceable.Objects.Remote.Proxying
{
    public static class ProxyContextExtensions
    {
        public static TResultType Execute<T, TResultType, TExecutionContext>(
            this TExecutionContext context,
            IRemotableCommand<T, TResultType> command)
            where T : Context<T>
            where TExecutionContext : Context<TExecutionContext>, ITypeSafeProxyContext
        {
            var remotableCarrier = context.CreateRemotableCarrier<TExecutionContext, T, TResultType>();
            remotableCarrier.RemotableCommand = command;
            return context.Execute(remotableCarrier);
        }

        public static TExecutionContext Execute<T, TResultType, TExecutionContext>(
            this TExecutionContext context,
            IReproducibleCommand<T, TResultType> command)
            where TExecutionContext : Context<TExecutionContext>, ITypeSafeProxyContext
            where T : Context<T>
        {
            var reproducibleCarrier = context.CreateReproducibleCarrier<TExecutionContext, T, TResultType>();
            reproducibleCarrier.ReproducibleCommand = command;
            return context.Execute(reproducibleCarrier);
        }
    }
}
