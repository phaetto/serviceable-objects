namespace TestHttpCompositionConsoleApp.Contexts.Queues.Commands
{
    using Data;
    using Serviceable.Objects.Remote;

    public sealed class Enqueue : ReproducibleCommandWithData<QueueContext, QueueContext, QueueItem>
    {
        public Enqueue(QueueItem data) : base(data)
        {
        }

        public override QueueContext Execute(QueueContext context)
        {
            context.QueueStorage.Enqueue(Data);
            return context;
        }
    }
}
