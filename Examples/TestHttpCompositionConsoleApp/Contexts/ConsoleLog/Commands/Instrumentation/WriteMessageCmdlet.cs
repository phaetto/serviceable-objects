using System.Management.Automation;

namespace TestHttpCompositionConsoleApp.Contexts.ConsoleLog.Commands.Instrumentation
{
    using Serviceable.Objects.Instrumentation.Powershell;

    [Cmdlet("Write", "Message")]
    public sealed class WriteMessageCmdlet : InstrumentationCommandCmdlet<WriteMessage, WriteMessageData>
    {
    }
}
