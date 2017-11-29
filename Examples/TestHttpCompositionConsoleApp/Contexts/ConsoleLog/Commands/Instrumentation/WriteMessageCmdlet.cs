using System.Management.Automation;
using Serviceable.Objects.Instrumentation;

namespace TestHttpCompositionConsoleApp.Contexts.ConsoleLog.Commands.Instrumentation
{
    [Cmdlet("Write", "Message")]
    public sealed class WriteMessageCmdlet : InstrumentationCommandCmdlet<WriteMessage, WriteMessageData>
    {
    }
}
