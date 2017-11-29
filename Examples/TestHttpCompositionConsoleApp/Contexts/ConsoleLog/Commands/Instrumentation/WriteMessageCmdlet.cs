using System.Management.Automation;
using Serviceable.Objects.Instrumentation;

namespace TestHttpCompositionConsoleApp.Contexts.ConsoleLog.Commands.Instrumentation
{
    using Serviceable.Objects.Instrumentation.Powershell;

    [Cmdlet("Write", "Message")]
    public sealed class WriteMessageCmdlet : InstrumentationCommandCmdlet<WriteMessage, WriteMessageData>
    {
    }
}
