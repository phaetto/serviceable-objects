
namespace TestHttpCompositionConsoleApp
{
    using Serviceable.Objects.Remote.Composition.Host;
    using Serviceable.Objects.Remote.Composition.Host.Commands;

    class Program
    {
        static void Main(string[] args)
        {
            /*
             * Principles:
             * 
             * Testable
             * Composable
             * Configurable
             * Instrumentable
             * Scalable
             * Updatable TODO
             * 
             */

            new ApplicationHost(args).Execute(new RunAndBlock());
        }
    }
}