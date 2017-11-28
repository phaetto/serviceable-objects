namespace TestHttpCompositionConsoleApp.Contexts.ConsoleLog.Commands
{
    using Serviceable.Objects.Remote.Serialization;

    public class WriteMessageData : SerializableSpecification
    {
        public string Message { get; set; }

        public override int DataStructureVersionNumber => 1;
    }
}