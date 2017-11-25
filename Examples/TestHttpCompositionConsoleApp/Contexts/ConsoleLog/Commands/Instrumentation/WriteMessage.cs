using System;
using Serviceable.Objects.Remote;

namespace TestHttpCompositionConsoleApp.Contexts.ConsoleLog.Commands.Instrumentation
{
    public sealed class WriteMessage : ReproducibleCommand<ConsoleLogContext, ConsoleLogContext>
    {
        public override ConsoleLogContext Execute(ConsoleLogContext context)
        {
            Console.WriteLine("\n\nInstrumentation Command: *** Executed ***");

            return context;
        }
    }
}
