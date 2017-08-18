namespace TestHttpCompositionConsoleApp
{
    using Serviceable.Objects.Composition;
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
    GraphVertices: [
        { TypeFullName:'" + typeof(OwinHttpContext).FullName + @"', Id:'server-context' },
        { TypeFullName:'" + typeof(QueueContext).FullName + @"', Id:'queue-context', ParentId:'server-context' },
        { TypeFullName:'" + typeof(ConsoleLogContext).FullName + @"', Id:'log-context', ParentId:'queue-context' },
    ]
}
";
            
            var contextGraph = new ContextGraph();
            contextGraph.FromJson(configuration);
            contextGraph.Execute(new Run());
        }
    }
}