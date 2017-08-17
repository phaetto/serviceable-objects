namespace Serviceable.Objects.Tests.Classes
{
    public sealed class ActionForTestEventProducerCalledEvent : IEvent
    {
        public string LastValue { get; set; }
        public string ChangedTo { get; set; }
    }
}
