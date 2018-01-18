namespace TestHttpCompositionConsoleApp.Contexts.Queues
{
    using System.Collections.Generic;
    using Commands.Data;
    using Serviceable.Objects;

    public sealed class QueueContext : Context<QueueContext>
    {
        public Queue<QueueItem> QueueStorage = new Queue<QueueItem>();
    }
}
