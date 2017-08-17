namespace Serviceable.Objects.Remote
{
    public abstract class ReproducibleActionWithData<TContext, TReceived, TDataType> : ReproducibleActionWithSerializableData<TContext, TReceived, TDataType>
    {
        protected ReproducibleActionWithData(TDataType data) : base(data)
        {
        }
    }
}
