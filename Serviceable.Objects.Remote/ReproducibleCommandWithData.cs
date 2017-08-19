namespace Serviceable.Objects.Remote
{
    public abstract class ReproducibleCommandWithData<TContext, TReceived, TDataType> : ReproducibleCommandWithSerializableData<TContext, TReceived, TDataType>
    {
        protected ReproducibleCommandWithData(TDataType data) : base(data)
        {
        }
    }
}
