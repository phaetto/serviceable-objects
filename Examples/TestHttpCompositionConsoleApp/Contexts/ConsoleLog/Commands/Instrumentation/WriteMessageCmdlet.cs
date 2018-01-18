namespace TestHttpCompositionConsoleApp.Contexts.ConsoleLog.Commands.Instrumentation
{
    using System.Management.Automation;
    using Data;
    using Serviceable.Objects.Instrumentation.Powershell;

    [Cmdlet("Write", "Message")]
    public sealed class WriteMessageCmdlet : InstrumentationCommandCmdlet<WriteMessage, WriteMessageData>
    {
    }
}
