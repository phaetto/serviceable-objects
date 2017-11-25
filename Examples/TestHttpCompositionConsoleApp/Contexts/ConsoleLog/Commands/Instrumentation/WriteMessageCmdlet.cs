using System.Management.Automation;
using Serviceable.Objects.Instrumentation;

namespace TestHttpCompositionConsoleApp.Contexts.ConsoleLog.Commands.Instrumentation
{
    [Cmdlet("Write", "Message")]
    public sealed class WriteMessageCmdlet : InstrumentationCommandCmdlet<WriteMessage>
    {
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, Position = 0, HelpMessage = "The message you would like to print")]
        public string Message { get; set; }

        public override WriteMessage GenerateCommand()
        {
            return new WriteMessage();
        }
    }
}
