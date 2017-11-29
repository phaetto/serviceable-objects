namespace TestHttpCompositionConsoleApp.Contexts.Queues.Commands.Instrumentation
{
    using System.Management.Automation;
    using Serviceable.Objects.Instrumentation;

    [Cmdlet("Dequeue", "Message")]
    public sealed class DequeueCmdlet : InstrumentationCommandCmdlet<Dequeue>
    {
    }
}