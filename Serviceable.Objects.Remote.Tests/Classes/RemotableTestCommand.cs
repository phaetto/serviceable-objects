namespace Serviceable.Objects.Remote.Tests.Classes
{
    using System.Collections.Generic;
    using Objects.Tests.Classes;

    public class RemotableTestCommand : RemotableCommandWithData<ReproducibleTestData, ReproducibleTestData, ContextForTest>, IEventProducer
    {
        public RemotableTestCommand(ReproducibleTestData testData)
            : base(testData)
        {
            Data.DomainName = "Starting value";
        }

        public override ReproducibleTestData Execute(ContextForTest context)
        {
            context.ContextVariable = Data.ChangeToValue;
            return Data;
        }

        public List<IEvent> EventsProduced { get; } = new List<IEvent>();
    }
}
