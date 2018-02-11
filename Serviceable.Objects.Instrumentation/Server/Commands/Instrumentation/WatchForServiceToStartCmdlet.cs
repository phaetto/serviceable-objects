namespace Serviceable.Objects.Instrumentation.Server.Commands.Instrumentation
{
    using System.Management.Automation;
    using Powershell;

    [Cmdlet(VerbsCommon.Watch, "ServiceUntilStarts")]
    public sealed class WatchForServiceToStartCmdlet : InstrumentationCommandCmdlet<WaitForServiceToStart>
    {
    }
}
