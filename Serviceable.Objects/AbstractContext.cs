namespace Serviceable.Objects
{
    using System.Collections.Generic;

    public abstract class AbstractContext
    {
        public event CommandEventWithResultDelegate CommandEventWithResultPublished;
        public event CommandEventDelegate CommandEventPublished;

        protected virtual void PublishCommandEvent(IEvent eventpublished)
        {
            CommandEventPublished?.Invoke(eventpublished);
        }

        protected virtual IEnumerable<EventResult> PublishCommandEventAndGetResults(IEvent eventpublished)
        {
            return CommandEventWithResultPublished?.Invoke(eventpublished);
        }
    }
}
