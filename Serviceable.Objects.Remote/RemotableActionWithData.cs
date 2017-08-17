namespace Serviceable.Objects.Remote
{
    public abstract class RemotableActionWithData<TSend, TReceived, TContext> : RemotableActionWithSerializableData<TSend, TReceived, TContext>
    {
        protected RemotableActionWithData(TSend data): base(data)
        {
        }

        public abstract override TReceived Execute(TContext context);
    }
}