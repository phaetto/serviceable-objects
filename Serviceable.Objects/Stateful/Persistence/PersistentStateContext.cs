namespace Serviceable.Objects.Stateful.Persistence
{
    using System;
    using Exceptions;

    public abstract class PersistentStateContext<TData, TContextType> : Context<TContextType>, IStatefulContext<TData, TContextType>
        where TContextType : Context<TContextType>
        where TData : struct
    {
        private readonly IPersistentStore<TData> persistentStore;

        protected PersistentStateContext(IPersistentStore<TData> persistentStore)
            : this(persistentStore.New(), persistentStore)
        {
        }

        protected PersistentStateContext(TData state, IPersistentStore<TData> persistentStore)
        {
            State = state;
            this.persistentStore = persistentStore;

            var lastDateTime = persistentStore.GetLastSavedTime(state);
            if (lastDateTime == null)
            {
                persistentStore.Save(state);
            }
            else
            {
                State = persistentStore.Load(State);
            }
        }

        protected override TReturnType InvokeExecute<TReturnType>(ICommand<TContextType, TReturnType> action)
        {
            var result = base.InvokeExecute(action);

            var lastDateTime = persistentStore.GetLastSavedTime(State);
            if (lastDateTime > DateTimeOffset.UtcNow)
            {
                throw new DataIntegrityViolationException();
            }
            persistentStore.Save(State);

            return result;
        }

        public TData State { get; }
    }
}
