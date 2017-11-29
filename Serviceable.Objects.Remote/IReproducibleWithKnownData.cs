namespace Serviceable.Objects.Remote
{
    public interface IReproducibleWithKnownData<TDataType> : IReproducibleWithData
    {
        TDataType Data { get; set; }
    }
}