﻿namespace Serviceable.Objects.Composition.Service
{
    using System.Collections.Generic;
    using Dependencies;
    using Graph;
    using ServiceOrchestrator;

    public interface IService
    {
        string OrchestratorName { get; }
        string ServiceName { get; }
        string TemplateName { get; }
        IList<InBinding> InBindings { get; }
        IList<ExternalBinding> ExternalBindings { get; }
        Container ServiceContainer { get; }
        GraphContext GraphContext { get; }
    }
}