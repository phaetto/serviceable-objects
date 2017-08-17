namespace Serviceable.Objects.Remote
{
    public interface IRemotableAction<in TContext, out TReceived> : ICommand<TContext, TReceived>, IRemotable
    {
    }
}
