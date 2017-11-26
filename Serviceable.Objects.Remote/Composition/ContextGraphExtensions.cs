using System.Linq;

namespace Serviceable.Objects.Remote.Composition
{
    using Objects.Composition.Graph;
    using Serviceable.Objects.Composition;
    using Serviceable.Objects.Remote.Dependencies;
    using Serviceable.Objects.Remote.Serialization;

    public static class ContextGraphExtensions
    {
        public static void FromJson(this ContextGraph contextGraph, string json)
        {
            var specification = DeserializableSpecification<GraphSpecification>.DeserializeFromJson(json);

            foreach (var graphNode in specification.GraphNodes)
            {
                if (specification.GraphVertices.All(x => x.ToId != graphNode.Id))
                {
                    contextGraph.AddInput(Types.FindType(graphNode.TypeFullName), graphNode.Id);
                }
                else
                {
                    contextGraph.AddNode(Types.FindType(graphNode.TypeFullName), graphNode.Id);
                }
            }

            foreach (var graphVertex in specification.GraphVertices)
            {
                contextGraph.ConnectNodes(graphVertex.FromId, graphVertex.ToId);
            }
        }
    }
}
