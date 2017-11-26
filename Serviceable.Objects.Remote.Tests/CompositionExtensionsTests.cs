namespace Serviceable.Objects.Remote.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Objects.Composition.Graph;
    using Serviceable.Objects.Composition;
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
                GraphNodes = new []
                {
                    new GraphNode()
                    {
                        Id = "node-1",
                        TypeFullName = typeof(ContextForTest).FullName,
                    },
                    new GraphNode()
                    {
                        Id = "node-2",
                        TypeFullName = typeof(ContextForTest2).FullName,
                    },
                    new GraphNode()
                    {
                        Id = "node-3",
                        TypeFullName = typeof(ContextForTest3).FullName,
                    },
                    // Hooks up on the node-2 node and listens to the commands running
                    new GraphNode()
                    {
                        Id = "node-assert",
                        TypeFullName = typeof(AssertNode).FullName,
                    },
                },
                GraphVertices = new []
                {
                    new GraphVertex
                    {
                        FromId = "node-1",
                        ToId = "node-2",
                    },
                    new GraphVertex
                    {
                        FromId = "node-1",
                        ToId = "node-3",
                    },
                    new GraphVertex
                    {
                        FromId = "node-2",
                        ToId = "node-assert",
                    },
                },
            };

            var json = graphSpec.SerializeToJson();

            var graph = new ContextGraph();
            graph.FromJson(json);

            var resultStacks = graph.Execute(new ActionForTestEventProducer("new-value")).ToList();

            Assert.Single(resultStacks);
            Assert.Equal(2, resultStacks[0].Count);
            Assert.Equal(typeof(ContextForTest), resultStacks[0].ElementAt(0).ContextType);
            Assert.Equal(typeof(ContextForTest2), resultStacks[0].ElementAt(1).ContextType);
        }

        private sealed class AssertNode : Context<AssertNode>, IPostGraphFlowPullControl
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
