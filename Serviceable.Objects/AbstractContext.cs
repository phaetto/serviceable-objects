namespace Serviceable.Objects
{
    using System.Collections.Generic;

    public abstract class AbstractContext
    {
        public event CommandEventWithResultDelegate CommandEventWithResultPublished;
        public event CommandEventDelegate CommandEventPublished;

        protected virtual void OnCommandEventPublished(IEvent eventpublished)
        {
            CommandEventPublished?.Invoke(eventpublished);
        }

        protected virtual IEnumerable<EventResult> OnCommandEventWithResultPublished(IEvent eventpublished)
        {
            return CommandEventWithResultPublished?.Invoke(eventpublished);
        }
    }
}
