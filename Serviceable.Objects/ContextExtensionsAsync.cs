namespace Serviceable.Objects
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public static class ContextExtensionsAsync
    {
        public static Task<TReturnChainType> Execute<T, TReturnChainType>(
            this Task<T> context,
            ICommand<T, Task<TReturnChainType>> action)
            where T : Context<T>
        {
            return context.ContinueWith(x => x.Result.Execute(action)).Unwrap();
        }

        public static Task<TReturnChainType> Execute<T, TReturnChainType>(
            this T context,
            ICommand<T, Task<TReturnChainType>> action)
            where T : Context<T>
        {
            return context.Execute(action);
        }

        public static Task<TReturnChainType> Execute<T, TReturnChainType>(
            this Task<T> context,
            ICommand<T, TReturnChainType> action)
            where T : Context<T>
        {
            return context.ContinueWith(x => x.Result.Execute(action));
        }

        public static Task<IEnumerable<TReturnChainType>> Execute<T, TReturnChainType>(
            this Task<T> context,
            IEnumerable<ICommand<T, TReturnChainType>> actions)
            where T : Context<T>
        {
            return context.ContinueWith(x => x.Result.Execute(actions));
        }

        public static Task<TReturnChainType> ForceExecuteAsync<T, TReturnChainType>(
            this Task<T> context,
            ICommand<T, TReturnChainType> action)
            where T : Context<T>
        {
            return context.ContinueWith(x => x.Result.Execute(action));
        }
    }
}
