namespace Serviceable.Objects.Stateful.Persistence
{
    using System;

    public interface IPersistentEventStore<in T>
        where T : IEvent
    {
        DateTimeOffset? GetLastEventTime(T data);

        IEvent[] LoadAppliedEvents(T data, DateTimeOffset? fromDateTimeOffset = null);

        void CreateEvent(T data);

        void AppendEvent(T data);

        void DeleteEvent(T data);
    }
}