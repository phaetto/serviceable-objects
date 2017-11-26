namespace TestHttpCompositionConsoleApp.Contexts.ConsoleLog.Commands
{
    using System;
    using Serviceable.Objects.Remote;

    public sealed class WriteMessage : ReproducibleCommand<ConsoleLogContext, ConsoleLogContext>
    {
        public override ConsoleLogContext Execute(ConsoleLogContext context)
        {
            Console.WriteLine("\nInstrumentation Command: *** Executed ***");

            return context;
        }
    }
}
