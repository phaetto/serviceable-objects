namespace Serviceable.Objects
{
    using System.Collections.Generic;

    // Use IList as it forces the user of the event to enumerate before sending the results back
    public delegate IList<EventResult> CommandEventDelegate(IEvent eventPublished);
}
