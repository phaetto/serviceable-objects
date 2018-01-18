namespace Serviceable.Objects.Instrumentation.Composition.ServiceOrchestrator.Commands.Instrumentation
{
    using System.Management.Automation;
    using Powershell;
    using Remote.Composition.ServiceOrchestrator.Commands;

    [Cmdlet(VerbsCommon.Close, "Orchestrator")]
    public sealed class CloseOrchestratorCmdlet : InstrumentationCommandCmdlet<CloseOrchestrator>
    {
    }
}
