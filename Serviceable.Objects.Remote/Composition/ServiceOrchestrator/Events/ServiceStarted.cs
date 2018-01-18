namespace Serviceable.Objects.Remote.Composition.ServiceOrchestrator.Events
{
    public sealed class ServiceStarted : IEvent
    {
        public int ProcessId { get; set; }
        public string ServiceName { get; set; }
    }
}
