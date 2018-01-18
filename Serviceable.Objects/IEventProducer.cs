namespace Serviceable.Objects
{
    using System.Collections.Generic;

    public interface IEventProducer
    {
        List<IEvent> EventsProduced { get; }
    }
}
