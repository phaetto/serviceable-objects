namespace Serviceable.Objects.Instrumentation.Composition.ServiceOrchestrator.Commands
{
    using System.Management.Automation;
    using Powershell;
    using Remote.Composition.ServiceOrchestrator.Commands;

    [Cmdlet(VerbsCommon.Get, "InBinding")]
    public sealed class GetInBindingCmdlet : InstrumentationCommandCmdlet<GetInBinding, string>
    {
    }
}