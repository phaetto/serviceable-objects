
namespace TestHttpCompositionConsoleApp
{
    using Serviceable.Objects.Composition;
    using Serviceable.Objects.IO.NamedPipes;
    using Serviceable.Objects.Remote.Composition;
    using TestHttpCompositionConsoleApp.Contexts.ConsoleLog;
    using TestHttpCompositionConsoleApp.Contexts.Http;
    using TestHttpCompositionConsoleApp.Contexts.Http.Commands;
    using TestHttpCompositionConsoleApp.Contexts.Queues;

    class Program
    {
        static void Main(string[] args)
        {
            var configuration = @"
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

            var contextGraph = new ContextGraph();
            contextGraph.FromJson(configuration);
            contextGraph.Execute(new Run());
        }
    }
}