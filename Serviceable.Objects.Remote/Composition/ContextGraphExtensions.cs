namespace Serviceable.Objects.Remote.Composition
{
    using Serviceable.Objects.Composition;
    using Serviceable.Objects.Remote.Dependencies;
    using Serviceable.Objects.Remote.Serialization;

    public static class ContextGraphExtensions
    {
        public static void FromJson(this ContextGraph contextGraph, string json)
        {
            var specification = DeserializableSpecification<GraphSpecification>.DeserializeFromJson(json);

            foreach (var graphVertex in specification.GraphVertices)
            {
                if (!string.IsNullOrWhiteSpace(graphVertex.ParentId))
                {
                    contextGraph.AddNode(Types.FindType(graphVertex.TypeFullName), graphVertex.Id, graphVertex.ParentId);
                }
                else
                {
                    contextGraph.AddRoot(Types.FindType(graphVertex.TypeFullName), graphVertex.Id);
                }
            }
        }
    }
}
