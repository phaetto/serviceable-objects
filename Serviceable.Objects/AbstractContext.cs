namespace Serviceable.Objects
{
    using System.Collections.Generic;

    public abstract class AbstractContext
    {
        public event ContextEventDelegate ContextEventPublished;
        public event CommandEventDelegate CommandEventPublished;

        protected IList<EventResult> PublishCommandEvent(IEvent eventpublished)
        {
            return CommandEventPublished?.Invoke(eventpublished);
        }

        protected IList<EventResult> PublishContextEvent(IEvent eventpublished)
        {
            return ContextEventPublished?.Invoke(eventpublished);
        }
    }
}
