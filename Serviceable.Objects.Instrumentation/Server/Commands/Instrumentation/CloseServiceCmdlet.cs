namespace Serviceable.Objects.Instrumentation.Server.Commands.Instrumentation
{
    using System.Management.Automation;
    using Powershell;

    [Cmdlet(VerbsCommon.Close, "Service")]
    public sealed class CloseServiceCmdlet : InstrumentationCommandCmdlet<CloseService>
    {
    }
}
