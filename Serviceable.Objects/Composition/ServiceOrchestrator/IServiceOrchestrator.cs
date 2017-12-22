﻿namespace Serviceable.Objects.Composition.ServiceOrchestrator
{
    using System.Collections.Generic;
    using Dependencies;

    public interface IServiceOrchestrator
    {
        IList<ServiceRegistration> ServiceRegistrations { get; }
        string OrchestratorName { get; }
        Container ServiceOrchestratorContainer { get; }
        Binding Binding { get; }
        ExternalBinding ExternalBinding { get; }
        IDictionary<string, string> GraphTemplatesDictionary { get; }
    }
}