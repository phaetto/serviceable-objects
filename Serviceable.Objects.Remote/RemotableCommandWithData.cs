namespace Serviceable.Objects.Remote
{
    public abstract class RemotableCommandWithData<TSend, TReceived, TContext> : RemotableCommandWithSerializableData<TSend, TReceived, TContext>
    {
        protected RemotableCommandWithData(TSend data): base(data)
        {
        }

        public abstract override TReceived Execute(TContext context);
    }
}