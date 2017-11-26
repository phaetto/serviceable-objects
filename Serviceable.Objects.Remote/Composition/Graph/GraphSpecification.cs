namespace Serviceable.Objects.Remote.Composition.Graph
{
    using System.Collections.Generic;
    using Serialization;

    public sealed class GraphSpecification : SerializableSpecification
    {
        public override int DataStructureVersionNumber => 1;

        public IEnumerable<GraphNode> GraphNodes { get; set; }

        public IEnumerable<GraphVertex> GraphVertices { get; set; }
    }
}
