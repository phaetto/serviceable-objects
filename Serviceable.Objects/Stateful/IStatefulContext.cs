namespace Serviceable.Objects.Stateful
{
    public interface IStatefulContext<out TState, TContextType>
        where TState : struct
        where TContextType : Context<TContextType>
    {
        TState State { get; }
    }
}