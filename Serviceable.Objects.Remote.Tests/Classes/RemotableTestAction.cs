namespace Serviceable.Objects.Remote.Tests.Classes
{
    using System.Collections.Generic;
    using Serviceable.Objects.Tests.Classes;

    public class RemotableTestAction : RemotableActionWithData<ReproducibleTestData, ReproducibleTestData, ContextForTest>, IEventProducer
    {
        public RemotableTestAction(ReproducibleTestData testData)
            : base(testData)
        {
            Data.DomainName = "Starting value";
        }

        public override ReproducibleTestData Execute(ContextForTest context)
        {
            context.ContextVariable = Data.ChangeToValue;
            return Data;
        }

        public IEnumerable<IEvent> EventsProduced { get; set; }
    }
}
