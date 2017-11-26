
namespace TestHttpCompositionConsoleApp
{
    using Contexts.ConfigurationSource;
    using Serviceable.Objects.Composition.Graph;
    using Serviceable.Objects.Dependencies;
    using Serviceable.Objects.IO.NamedPipes;
    using Serviceable.Objects.Remote.Composition.Graph;
    using TestHttpCompositionConsoleApp.Contexts.ConsoleLog;
    using TestHttpCompositionConsoleApp.Contexts.Http;
    using TestHttpCompositionConsoleApp.Contexts.Http.Commands;
    using TestHttpCompositionConsoleApp.Contexts.Queues;

    class Program
    {
        static void Main(string[] args)
        {
            var graphTemplate = @"
{
    GraphNodes: [
        { TypeFullName:'" + typeof(OwinHttpContext).FullName + @"', Id:'server-context' },
        { TypeFullName:'" + typeof(QueueContext).FullName + @"', Id:'queue-context' },
        { TypeFullName:'" + typeof(ConsoleLogContext).FullName + @"', Id:'log-context' },
        { TypeFullName:'" + typeof(NamedPipeServerContext).FullName + @"', Id:'named-pipe-instrumentation-context' },
        /*{ TypeFullName:'" + typeof(NamedPipeServerContext).FullName + @"', Id:'instrumentation-context' },*/
    ],
    GraphVertices: [
        { FromId:'server-context', ToId:'queue-context', },
        { FromId:'queue-context', ToId:'log-context',  },
        { FromId:'named-pipe-instrumentation-context', ToId:'log-context',  },
        /*{ FromId:'named-pipe-instrumentation-context', ToId:'instrumentation-context',  },*/
    ],
}
";
            /*
             * Principles:
             * 
             * Composable
             * Configurable
             * Testable
             * Intrumentable
             * 
             */

            // TODO: Add service (graph instance + template + name) extensions with container extensions (on graph or in service-container?)
            // TODO: Add service-container (service and process orchestrator)
            // TODO: Add host for service-containers (bring-your-own-process logic)

            var container = new Container();
            container.RegisterWithDefaultInterface(typeof(MemoryConfigurationSource));

            var contextGraph = new ContextGraph(container);
            container.Register(contextGraph); // Make the graph accessible in this container // TODO: is it applicable in every scenario?

            contextGraph.FromJson(graphTemplate);
            contextGraph.Configure();
            contextGraph.Initialize();
            contextGraph.Execute(new Run()); // TODO: blocking operation must be on Host - owin must run on a task
        }
    }
}