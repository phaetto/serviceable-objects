namespace Serviceable.Objects
{
    using System.Collections.Generic;

    public delegate IEnumerable<EventResult> CommandEventWithResultDelegate(IEvent eventPublished);
}