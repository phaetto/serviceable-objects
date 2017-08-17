namespace Serviceable.Objects.Remote.Proxying
{
    using System.Threading.Tasks;

    public static class ProxyContextExtensionsAsync
    {
        public static Task<TResultType> Execute<T, TResultType, TExecutionContext>(
            this Task<TExecutionContext> context,
            IRemotableAction<T, TResultType> action)
            where T : Context<T>
            where TExecutionContext : Context<TExecutionContext>, IProxyContext
        {
            return context.ContinueWith(x =>
            {
                var remotableCarrier = x.Result.CreateRemotableCarrier<TExecutionContext, T, TResultType>();
                remotableCarrier.RemotableAction = action;
                return x.Result.Execute(remotableCarrier);
            });
        }

        public static Task<TExecutionContext> Execute<T, TResultType, TExecutionContext>(
            this Task<TExecutionContext> context,
            IReproducibleAction<T, TResultType> action)
            where TExecutionContext : Context<TExecutionContext>, IProxyContext
            where T : Context<T>
        {
            return context.ContinueWith(x =>
            {
                var reproducibleCarrier = x.Result.CreateReproducibleCarrier<TExecutionContext, T, TResultType>();
                reproducibleCarrier.ReproducibleAction = action;
                return x.Result.Execute(reproducibleCarrier);
            });
        }

        public static Task<TResultType> Execute<T, TResultType, TExecutionContext>(
            this Task<TExecutionContext> context,
            IRemotableAction<T, Task<TResultType>> action)
            where T : Context<T>
            where TExecutionContext : Context<TExecutionContext>, IProxyContext
        {
            return context.ContinueWith(x =>
            {
                var remotableCarrier = x.Result.CreateRemotableCarrier<TExecutionContext, T, Task<TResultType>>();
                remotableCarrier.RemotableAction = action;
                return x.Result.Execute(remotableCarrier);
            }).Unwrap();
        }

        public static Task<TExecutionContext> Execute<T, TResultType, TExecutionContext>(
            this Task<TExecutionContext> context,
            IReproducibleAction<T, Task<TResultType>> action)
            where TExecutionContext : Context<TExecutionContext>, IProxyContext
            where T : Context<T>
        {
            return context.ContinueWith(x =>
            {
                var reproducibleCarrier = x.Result.CreateReproducibleCarrier<TExecutionContext, T, Task<TResultType>>();
                reproducibleCarrier.ReproducibleAction = action;
                return x.Result.Execute(reproducibleCarrier);
            });
        }

        public static Task<TResultType> Execute<T, TResultType, TExecutionContext>(
            this TExecutionContext context,
            IRemotableAction<T, Task<TResultType>> action)
            where T : Context<T>
            where TExecutionContext : Context<TExecutionContext>, IProxyContext
        {
            var remotableCarrier = context.CreateRemotableCarrier<TExecutionContext, T, Task<TResultType>>();
            remotableCarrier.RemotableAction = action;
            return context.Execute(remotableCarrier);
        }

        public static Task<TExecutionContext> Execute<T, TResultType, TExecutionContext>(
            this TExecutionContext context,
            IReproducibleAction<T, Task<TResultType>> action)
            where TExecutionContext : Context<TExecutionContext>, IProxyContext
            where T : Context<T>
        {
            var reproducibleCarrier = context.CreateAsyncReproducibleCarrier<TExecutionContext, T, Task<TResultType>>();
            reproducibleCarrier.ReproducibleAction = action;
            return context.Execute(reproducibleCarrier);
        }
    }
}
