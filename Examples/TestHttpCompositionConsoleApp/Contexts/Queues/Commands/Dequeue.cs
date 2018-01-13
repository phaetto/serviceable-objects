namespace TestHttpCompositionConsoleApp.Contexts.Queues.Commands
{
    using Data;
    using Serviceable.Objects.Remote;

    public sealed class Dequeue : RemotableCommand<QueueItem, QueueContext>
    {
        public override QueueItem Execute(QueueContext context)
        {
            if (context.QueueStorage.Count == 0)
            {
                return null;
            }

            return context.QueueStorage.Dequeue();
        }
    }
}
