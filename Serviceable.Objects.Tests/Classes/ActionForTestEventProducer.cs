namespace Serviceable.Objects.Tests.Classes
{
    using System.Collections.Generic;

    public class ActionForTestEventProducer: ICommand<ContextForTest, ContextForTest>, IEventProducer
    {
        public readonly string ValueToChangeTo;

        public ActionForTestEventProducer(string valueToChangeTo)
        {
            ValueToChangeTo = valueToChangeTo;
        }

        public ContextForTest Execute(ContextForTest context)
        {
            var lastValue = context.ContextVariable;

            // Do changes
            context.ContextVariable = ValueToChangeTo;

            EventsProduced = new IEvent[]
            {
                new ActionForTestEventProducerCalledEvent
                {
                    ChangedTo = context.ContextVariable,
                    LastValue = lastValue,
                }
            };
            return context;
        }

        public IEnumerable<IEvent> EventsProduced { get; set; }
    }
}
