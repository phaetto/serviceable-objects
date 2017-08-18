namespace Serviceable.Objects.Remote.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Serviceable.Objects.Composition;
    using Serviceable.Objects.Dependencies;
    using Serviceable.Objects.Remote.Composition;
    using Serviceable.Objects.Tests.Classes;
    using Xunit;

    public sealed class CompositionExtensionsTests
    {
        [Fact]
        public void FromJson_WhenExecutingAGraphFromJson_ThenItSuccessfullyGetsNodeResponses()
        {
            var graphSpec = new GraphSpecification
            {
                GraphVertices = new []
                {
                    new GraphNodeWithVertex
                    {
                        Id = "node-1",
                        TypeFullName = typeof(ContextForTest).FullName,
                    },
                    new GraphNodeWithVertex
                    {
                        Id = "node-2",
                        ParentId = "node-1",
                        TypeFullName = typeof(ContextForTest2).FullName,
                    },
                    new GraphNodeWithVertex
                    {
                        Id = "node-3",
                        ParentId = "node-1",
                        TypeFullName = typeof(ContextForTest3).FullName,
                    },
                    // Hooks up on the node-2 node and listens to the commands running
                    new GraphNodeWithVertex
                    {
                        Id = "node-assert",
                        ParentId = "node-2",
                        TypeFullName = typeof(PullController).FullName,
                    },
                }
            };

            var json = graphSpec.SerializeToJson();

            var container = new Container();
            var graph = new ContextGraph(container);
            graph.FromJson(json);

            var resultStacks = graph.Execute(new ActionForTestEventProducer("new-value")).ToList();

            Assert.Single(resultStacks);
            Assert.Equal(2, resultStacks[0].Count);
            Assert.Equal(typeof(ContextForTest), resultStacks[0].ElementAt(0).ContextType);
            Assert.Equal(typeof(ContextForTest2), resultStacks[0].ElementAt(1).ContextType);
        }

        private sealed class PullController : Context<PullController>, IPostGraphFlowPullControl
        {
            public void PullNodeExecutionInformation(ContextGraph contextGraph, string executingNodeId, dynamic parentContext,
                dynamic parentCommandApplied, Stack<EventResult> eventResults)
            {
                var contextForTest2 = (ContextForTest2) parentContext;
                Assert.Equal("new-value", contextForTest2.ContextVariable);
            }
        }
    }
}
