namespace TestHttpCompositionConsoleApp.Contexts.ConsoleLog.Commands
{
    using System;
    using Data;
    using Serviceable.Objects.Remote;

    public sealed class WriteMessage : ReproducibleCommandWithData<ConsoleLogContext, ConsoleLogContext, WriteMessageData>
    {
        public WriteMessage(WriteMessageData data) : base(data)
        {
        }

        public override ConsoleLogContext Execute(ConsoleLogContext context)
        {
            Console.WriteLine(Data.Message);

            return context;
        }
    }
}
