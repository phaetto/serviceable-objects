namespace Serviceable.Objects.Remote.Tests
{
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
                    new GraphVertex
                    {
                        Id = "node-1",
                        TypeFullName = typeof(ContextForTest).FullName,
                    },
                    new GraphVertex
                    {
                        Id = "node-2",
                        ParentId = "node-1",
                        TypeFullName = typeof(ContextForTest2).FullName,
                    },
                    new GraphVertex
                    {
                        Id = "node-3",
                        ParentId = "node-1",
                        TypeFullName = typeof(ContextForTest3).FullName,
                    },
                }
            };

            var json = graphSpec.SerializeToJson();

            var container = new Container();
            var graph = new ContextGraph(container);
            graph.FromJson(json);

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
