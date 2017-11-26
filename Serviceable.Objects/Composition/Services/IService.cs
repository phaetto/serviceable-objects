namespace Serviceable.Objects.Composition.Services
{
    using Dependencies;
    using Graph;

    public interface IService
    {
        string ContainerName { get; }
        string ServiceName { get; }
        string TemplateName { get; }
        Container ServiceContainer { get; }
        GraphContext GraphContext { get; }
    }
}