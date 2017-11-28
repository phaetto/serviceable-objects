namespace TestHttpCompositionConsoleApp.Contexts.Queues.Commands.Instrumentation
{
    using System.Management.Automation;
    using Data;
    using Serviceable.Objects.Instrumentation;

    [Cmdlet("Enqueue", "Message")]
    public sealed class EnqueueCmdlet : InstrumentationCommandCmdlet<Enqueue>
    {
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, Position = 0, HelpMessage = "The message you would like to enqueue")]
        public string Message { get; set; }

        public override Enqueue GenerateCommand()
        {
            return new Enqueue(new QueueItem { Data = Message });
        }
    }
}
