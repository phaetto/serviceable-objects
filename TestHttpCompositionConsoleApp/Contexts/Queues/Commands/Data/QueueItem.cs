namespace TestHttpCompositionConsoleApp.Contexts.Queues.Commands.Data
{
    using Serviceable.Objects.Remote.Serialization;

    public sealed class QueueItem : SerializableSpecification
    {
        public override int DataStructureVersionNumber => 1;

        public string Data { get; set; }
    }
}
