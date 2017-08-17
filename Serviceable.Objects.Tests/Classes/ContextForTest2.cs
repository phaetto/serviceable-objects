namespace Serviceable.Objects.Tests.Classes
{
    public class ContextForTest2 : Context<ContextForTest2>,
        IEventHandler<ActionForTestEventProducerCalledEvent>
    {
        public string ContextVariable = null;

        public dynamic GetCommandForEvent(ActionForTestEventProducerCalledEvent eventPublished)
        {
            return new ActionForTest2(eventPublished.ChangedTo);
        }
    }
}
