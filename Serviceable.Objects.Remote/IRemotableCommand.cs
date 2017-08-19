namespace Serviceable.Objects.Remote
{
    public interface IRemotableCommand<in TContext, out TReceived> : ICommand<TContext, TReceived>, IRemotable
    {
    }
}
