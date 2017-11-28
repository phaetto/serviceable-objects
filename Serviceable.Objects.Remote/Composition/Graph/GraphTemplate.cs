namespace Serviceable.Objects.Remote.Composition.Graph
{
    using System.Collections.Generic;

    public sealed class GraphTemplate
    {
        public IEnumerable<GraphNode> GraphNodes { get; set; }

        public IEnumerable<GraphVertex> GraphVertices { get; set; }
    }
}
