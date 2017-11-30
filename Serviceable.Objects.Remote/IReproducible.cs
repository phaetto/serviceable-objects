namespace Serviceable.Objects.Remote
{
    using Serviceable.Objects.Remote.Serialization;

    public interface IReproducible
    {
        CommandSpecification GetInstanceSpec();
    }
}