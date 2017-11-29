namespace TestHttpCompositionConsoleApp.Contexts.Queues.Commands.Instrumentation
{
    using System.Management.Automation;
    using Data;
    using Serviceable.Objects.Instrumentation;
    using Serviceable.Objects.Instrumentation.Powershell;

    [Cmdlet("Enqueue", "Message")]
    public sealed class EnqueueCmdlet : InstrumentationCommandCmdlet<Enqueue, QueueItem>
    {
    }
}
