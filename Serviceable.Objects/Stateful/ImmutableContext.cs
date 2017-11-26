namespace Serviceable.Objects.Stateful
{
    using System;
    using Exceptions;

    public abstract class ImmutableContext<TState, TContextType> : Context<TContextType>, IStatefulContext<TState, TContextType>
        where TState : struct
        where TContextType : Context<TContextType>
    {
        public TState State { get; private set; }
        public bool HasBeenInitialized { get; private set; }

        protected ImmutableContext()
        {
        }

        protected ImmutableContext(TState state)
        {
            HasBeenInitialized = true;
            State = state;
        }

        public void SetState(TState state)
        {
            Check.Argument(HasBeenInitialized, nameof(state), "The state has already been configured");

            State = state;
            HasBeenInitialized = true;
        }

        protected override TReturnedContextType InvokeExecute<TReturnedContextType>(ICommand<TContextType, TReturnedContextType> action)
        {
            if (!HasBeenInitialized)
            {
                throw new InvalidOperationException("The context has not been configured yet.");
            }

            return base.InvokeExecute(action);
        }
    }
}
