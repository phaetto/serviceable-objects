namespace Serviceable.Objects
{
    public interface IEventHandler<in T> where T: IEvent
    {
        dynamic GetCommandForEvent(T eventPublished);
    }
}
