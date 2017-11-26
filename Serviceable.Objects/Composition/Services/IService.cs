namespace Serviceable.Objects.Composition.Services
{
    using Dependencies;
    using Graph;

    public interface IService
    {
        string ServiceName { get; }
        GraphContext GraphContext { get; }
        string TemplateName { get; }
        Container ServiceContainer { get; }
    }
}