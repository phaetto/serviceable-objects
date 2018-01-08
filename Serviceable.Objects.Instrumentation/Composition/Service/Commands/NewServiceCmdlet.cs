namespace Serviceable.Objects.Instrumentation.Composition.Service.Commands
{
    using System.Management.Automation;
    using Powershell;
    using Remote.Composition.Service.Commands;

    [Cmdlet(VerbsCommon.Close, "Service")]
    public sealed class CloseServiceCmdlet : InstrumentationCommandCmdlet<CloseService>
    {
    }
}