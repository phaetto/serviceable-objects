namespace Serviceable.Objects.Instrumentation.Composition.ServiceOrchestrator.Commands.Instrumentation
{
    using System.Management.Automation;
    using Powershell;
    using Remote.Composition.ServiceOrchestrator.Commands;
    using Remote.Composition.ServiceOrchestrator.Commands.Data;

    [Cmdlet(VerbsCommon.New, "Service")]
    public sealed class NewServiceCmdlet : InstrumentationCommandCmdlet<NewService, NewServiceData>
    {
    }
}