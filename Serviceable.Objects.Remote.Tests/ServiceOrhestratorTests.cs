namespace Serviceable.Objects.Remote.Tests
{
    using System.Linq;
    using System.Threading.Tasks;
    using Objects.Tests.Classes;
    using Remote.Composition.Host;
    using Remote.Composition.Host.Commands;
    using Remote.Composition.Service;
    using Remote.Composition.ServiceOrchestrator;
    using Remote.Composition.ServiceOrchestrator.Configuration;
    using Xunit;

    public sealed class ServiceOrhestratorTests
    {
        [Fact]
        public async Task Execute_WhenUsingOrchestratorWithAGraph_ThenItSuccessfullyInstantiates()
        {
            var service = new ServiceContext();
            var graph = service.GraphContext;
            var orchestrator = new ServiceOrchestratorContext(
                new ServiceOrchestratorConfiguration {},
                graph
            );
            var contextForTest2 = new ContextForTest2();
            var contextForTest3 = new ContextForTest3();

            graph.AddInput(typeof(ContextForTest), "node-1");
            graph.AddNode(contextForTest2, "node-2");
            graph.AddNode(contextForTest3, "node-3");
            graph.AddNode(orchestrator, "orch");
            graph.ConnectNodes("node-1", "node-2");
            graph.ConnectNodes("node-1", "node-3");

            var task = new ApplicationHost(service).ForceExecuteAsync(new RunAndBlock());

            await Task.Delay(300);

            var executionDataResults = graph.Execute(new ActionForTestEventProducer("new-value")).ToList();

            Assert.Single(executionDataResults);
            Assert.NotNull(executionDataResults[0]);
            Assert.Null(executionDataResults[0].Exception);
            Assert.Equal(typeof(ContextForTest), executionDataResults[0].SingleContextExecutionResultWithInfo.ContextType);
            Assert.Equal("new-value", contextForTest2.ContextVariable);
            Assert.Null(contextForTest3.ContextVariable);
        }
    }
}
