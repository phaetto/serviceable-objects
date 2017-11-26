namespace Serviceable.Objects.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Composition.Graph;
    using Serviceable.Objects.Composition;
    using Serviceable.Objects.Dependencies;
    using Serviceable.Objects.Tests.Classes;
    using Xunit;

    public sealed class CompositionTests
    {
        [Fact]
        public void ContextGraph_WhenCreatingAGraph_ThenTheMainObjectDependenciesAreNotInsertedToContainer()
        {
            var customObjectsCache = new Dictionary<string, object>();
            var container = new Container(customObjectsCache);
            var graph = new ContextGraph(container);

            graph.AddInput(typeof(ContextForTest), "node-1");
            graph.AddNode(typeof(ContextForTest2), "node-2");
            graph.ConnectNodes("node-1", "node-2");

            Assert.Empty(customObjectsCache);
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

            var graph = new ContextGraph();
            var contextForTest2 = new ContextForTest2();
            var contextForTest3 = new ContextForTest3();

            graph.AddInput(typeof(ContextForTest), "node-1");
            graph.AddNode(contextForTest2, "node-2");
            graph.AddNode(contextForTest3, "node-3");
            graph.ConnectNodes("node-1", "node-2");
            graph.ConnectNodes("node-1", "node-3");

            var resultStacks = graph.Execute(new ActionForTestEventProducer("new-value")).ToList();

            Assert.Equal("new-value", contextForTest2.ContextVariable);
            Assert.Null(contextForTest3.ContextVariable);
            Assert.Single(resultStacks);
            Assert.Equal(2, resultStacks[0].Count);
            Assert.Equal(typeof(ContextForTest), resultStacks[0].ElementAt(0).ContextType);
            Assert.Equal(typeof(ContextForTest2), resultStacks[0].ElementAt(1).ContextType);
        }

        // TODO: add interface tests
        // IGraphFlowEventPushControl, IPostGraphFlowPullControl
    }
}
