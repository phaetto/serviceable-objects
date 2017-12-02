namespace TestHttpCompositionConsoleApp.Contexts.Queues
{
    using System.Collections.Generic;
    using Serviceable.Objects;
    using Commands.Data;

    public sealed class QueueContext : Context<QueueContext>
    {
        public Queue<QueueItem> QueueStorage = new Queue<QueueItem>();
    }
}
