namespace Serviceable.Objects.Instrumentation.Composition.ServiceOrchestrator.Commands.Instrumentation
{
    using System.Management.Automation;
    using Powershell;
    using Remote.Composition.ServiceOrchestrator.Commands;

    [Cmdlet(VerbsCommon.Get, "ExternalBinding")]
    public sealed class GetExternalBindingCmdlet : InstrumentationCommandCmdlet<GetExternalBinding, string>
    {
    }
}