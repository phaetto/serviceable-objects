namespace TestHttpCompositionConsoleApp.Contexts.Queues.Commands
{
    using System;
    using Serviceable.Objects.Remote;

    public sealed class MakeArtificialError : ReproducibleCommand<QueueContext, QueueContext>
    {
        public override QueueContext Execute(QueueContext context)
        {
            throw new NotSupportedException("This error was totally on purpose");
        }
    }
}
