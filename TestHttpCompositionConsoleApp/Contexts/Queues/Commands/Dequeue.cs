namespace TestHttpCompositionConsoleApp.Contexts.Queues.Commands
{
    using Serviceable.Objects.Remote;
    using TestHttpCompositionConsoleApp.Contexts.Queues.Commands.Data;

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
