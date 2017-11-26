namespace Serviceable.Objects.Composition.Services
{
    using Dependencies;
    using Graph;

    public interface IService
    {
        string ServiceName { get; }
        ContextGraph ContextGraph { get; }
        string TemplateName { get; }
        Container ServiceContainer { get; }
    }
}