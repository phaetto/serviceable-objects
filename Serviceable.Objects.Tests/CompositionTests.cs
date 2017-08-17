namespace Serviceable.Objects.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Serviceable.Objects.Composition;
    using Serviceable.Objects.Dependencies;
    using Serviceable.Objects.Tests.Classes;
    using Xunit;

    public sealed class CompositionTests
    {
        [Fact]
        public void ContextGraph_WhenCreatingAnGraph_ThenItIsSuccessfullyInjectingAllDependencies()
        {
            var customObjectsCache = new Dictionary<string, object>();
            var container = new Container(customObjectsCache);
            var graph = new ContextGraph(container);

            var rootGuid = graph.AddRoot(typeof(ContextForTest));
            graph.AddNode(typeof(ContextForTest2), rootGuid);

            Assert.Equal(2, customObjectsCache.Count);
        }

        [Fact]
        public void Execute_WhenExecutingAnGraph_ThenItSuccessfullyGetsNodeResponses()
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

            var container = new Container();
            var graph = new ContextGraph(container);

            var contextForTestServiceGuid = graph.AddRoot(typeof(ContextForTest));
            graph.AddNode(typeof(ContextForTest2), contextForTestServiceGuid);
            graph.AddNode(typeof(ContextForTest3), contextForTestServiceGuid);

            var resultStacks = graph.Execute(new ActionForTestEventProducer("new-value")).ToList();

            var contextForTest2 = container.Resolve<ContextForTest2>();
            var contextForTest3 = container.Resolve<ContextForTest3>();

            Assert.Equal("new-value", contextForTest2.ContextVariable);
            Assert.Null(contextForTest3.ContextVariable);
            Assert.Single(resultStacks);
            Assert.Equal(2, resultStacks[0].Count);
            Assert.Equal(typeof(ContextForTest), resultStacks[0].ElementAt(0).ContextType);
            Assert.Equal(typeof(ContextForTest2), resultStacks[0].ElementAt(1).ContextType);
        }
    }
}
