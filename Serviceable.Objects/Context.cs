namespace Serviceable.Objects
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Serviceable.Objects.Exceptions;

    public class Context<T> : AbstractContext where T : Context<T>
    {
        protected virtual TReturnedContextType InvokeExecute<TReturnedContextType>(ICommand<T, TReturnedContextType> action)
        {
            return action.Execute((T) this);
        }

        private TReturnedContextType InvokeExecuteAndPostEvents<TReturnedContextType>(ICommand<T, TReturnedContextType> action)
        {
            var result = InvokeExecute(action);

            CheckAndInvokeEvents(action);

            return result;
        }

        private void CheckAndInvokeEvents<TReturnedContextType>(ICommand<T, TReturnedContextType> action)
        {
            var eventProducer = action as IEventProducer;
            if (eventProducer?.EventsProduced == null)
            {
                return;
            }

            foreach (var eventProduced in eventProducer.EventsProduced)
            {
                PublishCommandEventAndGetResults(eventProduced);
                PublishCommandEvent(eventProduced);
            }
        }

        public TReturnedContextType Execute<TReturnedContextType>(
            ICommand<T, TReturnedContextType> action)
        {
            Check.ArgumentNull(action, nameof(action));

            var result = InvokeExecuteAndPostEvents(action);

            return result;
        }

        public IEnumerable<TReturnedContextType> Execute<TReturnedContextType>(
            IEnumerable<ICommand<T, TReturnedContextType>> actions)
        {
            Check.ArgumentNull(actions, nameof(actions));

            return actions.Select(InvokeExecuteAndPostEvents);
        }

        public Task<TReturnedContextType> ForceExecuteAsync<TReturnedContextType>(
            ICommand<T, TReturnedContextType> action)
        {
            Check.ArgumentNull(action, nameof(action));

            return Task.Factory.StartNew(() => InvokeExecuteAndPostEvents(action));
        }

        public Task<TReturnedContextType> ForceExecuteAsync<TReturnedContextType>(
            IEnumerable<ICommand<T, TReturnedContextType>> actions)
        {
            Check.ArgumentNull(actions, nameof(actions));

            return Task.Factory.StartNew(() =>
                {
                    var results = Execute(actions);
                    var lastResult = default(TReturnedContextType);
                    foreach (var result in results)
                    {
                        lastResult = result;
                    }

                    return lastResult;
                });
        }
    }
}
