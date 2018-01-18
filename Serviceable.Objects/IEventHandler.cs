namespace Serviceable.Objects
{
    public interface IEventHandler<in T> where T: IEvent
    {
        object GetCommandForEvent(T eventPublished);
    }
}
