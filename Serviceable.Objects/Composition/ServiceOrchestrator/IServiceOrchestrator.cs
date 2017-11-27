namespace Serviceable.Objects.Composition.ServiceOrchestrator
{
    using System.Collections.Generic;
    using Dependencies;

    public interface IServiceOrchestrator
    {
        IList<ServiceRegistration> ServiceRegistrations { get; }
        string OrchestratorName { get; }
        Container ServiceOrchestratorContainer { get; }
        Binding ServiceOrchestratorBinding { get; }
        IList<ExternalBinding> ExternalBindings { get; }
        IDictionary<string, string> GraphTemplatesDictionary { get; }
    }
}