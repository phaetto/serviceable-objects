namespace Serviceable.Objects.Remote.Composition.Graph
{
    using System.Linq;
    using Dependencies;
    using Objects.Composition.Graph;
    using Serialization;

    public static class ContextGraphExtensions
    {
        public static void FromJson(this GraphContext graphContext, string json)
        {
            var specification = DeserializableSpecification<GraphSpecification>.DeserializeFromJson(json);

            foreach (var graphNode in specification.GraphNodes)
            {
                if (specification.GraphVertices.All(x => x.ToId != graphNode.Id))
                {
                    graphContext.AddInput(Types.FindType(graphNode.TypeFullName), graphNode.Id);
                }
                else
                {
                    graphContext.AddNode(Types.FindType(graphNode.TypeFullName), graphNode.Id);
                }
            }

            foreach (var graphVertex in specification.GraphVertices)
            {
                graphContext.ConnectNodes(graphVertex.FromId, graphVertex.ToId);
            }
        }
    }
}
