namespace Serviceable.Objects
{
    public interface ICommand<in TContextType, out TReturnedContextType>
    {
        TReturnedContextType Execute(TContextType context);
    }
}
