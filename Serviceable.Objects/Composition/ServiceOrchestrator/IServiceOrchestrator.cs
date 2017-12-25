namespace Serviceable.Objects.Composition.ServiceOrchestrator
{
    using System.Collections.Generic;
    using Dependencies;

    public interface IServiceOrchestrator
    {
        IList<ServiceRegistration> ServiceRegistrations { get; }
        string OrchestratorName { get; }
        Container ServiceOrchestratorContainer { get; }
        IDictionary<string, string> GraphTemplatesDictionary { get; }
        IDictionary<string, List<InBinding>> InBindingsPerService { get; }
        IDictionary<string, List<ExternalBinding>> ExternalBindingsPerService { get; }
    }
}