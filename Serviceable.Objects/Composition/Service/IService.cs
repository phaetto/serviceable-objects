namespace Serviceable.Objects.Composition.Service
{
    using Dependencies;
    using Graph;
    using ServiceOrchestrator;

    public interface IService
    {
        string OrchestratorName { get; }
        string ServiceName { get; }
        string TemplateName { get; }
        Binding Binding { get; }
        ExternalBinding ExternalBinding { get; }
        Container ServiceContainer { get; }
        GraphContext GraphContext { get; }
    }
}