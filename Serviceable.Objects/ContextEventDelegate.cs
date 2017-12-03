namespace Serviceable.Objects
{
    using System.Collections.Generic;

    public delegate IList<EventResult> ContextEventDelegate(IEvent eventPublished);
}
