namespace Serviceable.Objects.Remote
{
    using Serialization;

    public interface IReproducible
    {
        CommandSpecification GetInstanceSpec();
    }
}