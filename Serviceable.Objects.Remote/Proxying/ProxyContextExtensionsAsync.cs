namespace Serviceable.Objects.Remote.Proxying
{
    using System.Threading.Tasks;

    public static class ProxyContextExtensionsAsync
    {
        public static Task<TResultType> Execute<T, TResultType, TExecutionContext>(
            this Task<TExecutionContext> context,
            IRemotableCommand<T, TResultType> command)
            where T : Context<T>
            where TExecutionContext : Context<TExecutionContext>, ITypeSafeProxyContext
        {
            return context.ContinueWith(x =>
            {
                var remotableCarrier = x.Result.CreateRemotableCarrier<TExecutionContext, T, TResultType>();
                remotableCarrier.RemotableCommand = command;
                return x.Result.Execute(remotableCarrier);
            });
        }

        public static Task<TExecutionContext> Execute<T, TResultType, TExecutionContext>(
            this Task<TExecutionContext> context,
            IReproducibleCommand<T, TResultType> command)
            where TExecutionContext : Context<TExecutionContext>, ITypeSafeProxyContext
            where T : Context<T>
        {
            return context.ContinueWith(x =>
            {
                var reproducibleCarrier = x.Result.CreateReproducibleCarrier<TExecutionContext, T, TResultType>();
                reproducibleCarrier.ReproducibleCommand = command;
                return x.Result.Execute(reproducibleCarrier);
            });
        }

        public static Task<TResultType> Execute<T, TResultType, TExecutionContext>(
            this Task<TExecutionContext> context,
            IRemotableCommand<T, Task<TResultType>> command)
            where T : Context<T>
            where TExecutionContext : Context<TExecutionContext>, ITypeSafeProxyContext
        {
            return context.ContinueWith(x =>
            {
                var remotableCarrier = x.Result.CreateRemotableCarrier<TExecutionContext, T, Task<TResultType>>();
                remotableCarrier.RemotableCommand = command;
                return x.Result.Execute(remotableCarrier);
            }).Unwrap();
        }

        public static Task<TExecutionContext> Execute<T, TResultType, TExecutionContext>(
            this Task<TExecutionContext> context,
            IReproducibleCommand<T, Task<TResultType>> command)
            where TExecutionContext : Context<TExecutionContext>, ITypeSafeProxyContext
            where T : Context<T>
        {
            return context.ContinueWith(x =>
            {
                var reproducibleCarrier = x.Result.CreateReproducibleCarrier<TExecutionContext, T, Task<TResultType>>();
                reproducibleCarrier.ReproducibleCommand = command;
                return x.Result.Execute(reproducibleCarrier);
            });
        }

        public static Task<TResultType> Execute<T, TResultType, TExecutionContext>(
            this TExecutionContext context,
            IRemotableCommand<T, Task<TResultType>> command)
            where T : Context<T>
            where TExecutionContext : Context<TExecutionContext>, ITypeSafeProxyContext
        {
            var remotableCarrier = context.CreateRemotableCarrier<TExecutionContext, T, Task<TResultType>>();
            remotableCarrier.RemotableCommand = command;
            return context.Execute(remotableCarrier);
        }

        public static Task<TExecutionContext> Execute<T, TResultType, TExecutionContext>(
            this TExecutionContext context,
            IReproducibleCommand<T, Task<TResultType>> command)
            where TExecutionContext : Context<TExecutionContext>, ITypeSafeProxyContext
            where T : Context<T>
        {
            var reproducibleCarrier = context.CreateAsyncReproducibleCarrier<TExecutionContext, T, Task<TResultType>>();
            reproducibleCarrier.ReproducibleCommand = command;
            return context.Execute(reproducibleCarrier);
        }
    }
}
