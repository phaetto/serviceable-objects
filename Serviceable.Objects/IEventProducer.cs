namespace Serviceable.Objects
{
    using System.Collections.Generic;

    public interface IEventProducer
    {
        IEnumerable<IEvent> EventsProduced { get; set; }
    }
}
