namespace Serviceable.Objects.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Composition.Graph;
    using Dependencies;
    using Classes;
    using Xunit;

    public sealed class GraphContextTests
    {
        [Fact]
        public void AddInputConnectNodes_WhenCreatingAGraph_ThenTheMainObjectDependenciesAreNotInsertedToContainer()
        {
            var customObjectsCache = new Dictionary<string, object>();
            var container = new Container(customObjectsCache);
            var graph = new GraphContext(container);

            graph.AddInput(typeof(ContextForTest), "node-1");
            graph.AddNode(typeof(ContextForTest2), "node-2");
            graph.ConnectNodes("node-1", "node-2");

            Assert.Single(customObjectsCache);
        }

        [Fact]
        public void Execute_WhenExecutingAGraph_ThenItSuccessfullyGetsNodeResponses()
        {
            /*
             * ActionForTestEventProducer 
             *  -> Applied to ContextForTest 
             *  -> Generates event ActionForTestEventProducerCalledEvent
             *  -> ContextForTest2 responds with command ActionForTest2
             *  -> ContextForTest3 does not respond
             *  -> ContextForTest2 responding command copies the value over to ContextForTest2
             *
             */

            var graph = new GraphContext();
            var contextForTest2 = new ContextForTest2();
            var contextForTest3 = new ContextForTest3();

            graph.AddInput(typeof(ContextForTest), "node-1");
            graph.AddNode(contextForTest2, "node-2");
            graph.AddNode(contextForTest3, "node-3");
            graph.ConnectNodes("node-1", "node-2");
            graph.ConnectNodes("node-1", "node-3");

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
