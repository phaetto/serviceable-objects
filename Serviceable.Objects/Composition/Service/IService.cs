namespace Serviceable.Objects.Composition.Service
{
    using Dependencies;
    using Graph;

    public interface IService
    {
        string OrchestratorName { get; }
        string ServiceName { get; }
        string TemplateName { get; }
        Container ServiceContainer { get; }
        GraphContext GraphContext { get; }
    }
}