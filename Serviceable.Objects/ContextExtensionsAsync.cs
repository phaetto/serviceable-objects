namespace Serviceable.Objects
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public static class ContextExtensionsAsync
    {
        public static Task<TReturnChainType> Execute<TContext, TReturnChainType>(
            this Task<TContext> context,
            ICommand<TContext, Task<TReturnChainType>> action)
            where TContext : Context<TContext>
        {
            return context.ContinueWith(x => x.Result.Execute(action)).Unwrap();
        }

        public static Task<TReturnChainType> Execute<TContext, TReturnChainType>(
            this TContext context,
            ICommand<TContext, Task<TReturnChainType>> action)
            where TContext : Context<TContext>
        {
            return context.Execute(action);
        }

        public static Task<TReturnChainType> Execute<TContext, TReturnChainType>(
            this Task<TContext> context,
            ICommand<TContext, TReturnChainType> action)
            where TContext : Context<TContext>
        {
            return context.ContinueWith(x => x.Result.Execute(action));
        }

        public static Task<IEnumerable<TReturnChainType>> Execute<TContext, TReturnChainType>(
            this Task<TContext> context,
            IEnumerable<ICommand<TContext, TReturnChainType>> actions)
            where TContext : Context<TContext>
        {
            return context.ContinueWith(x => x.Result.Execute(actions));
        }

        public static Task<TReturnChainType> ForceExecuteAsync<TContext, TReturnChainType>(
            this Task<TContext> context,
            ICommand<TContext, TReturnChainType> action)
            where TContext : Context<TContext>
        {
            return context.ContinueWith(x => x.Result.Execute(action));
        }
    }
}
