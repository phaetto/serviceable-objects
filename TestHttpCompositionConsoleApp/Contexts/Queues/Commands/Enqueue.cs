namespace TestHttpCompositionConsoleApp.Contexts.Queues.Commands
{
    using Serviceable.Objects.Remote;
    using TestHttpCompositionConsoleApp.Contexts.Queues.Commands.Data;

    public sealed class Enqueue : ReproducibleActionWithData<QueueContext, QueueContext, QueueItem>
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
