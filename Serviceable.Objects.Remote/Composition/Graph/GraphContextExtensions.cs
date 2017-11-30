namespace Serviceable.Objects.Remote.Composition.Graph
{
    using System.Linq;
    using Dependencies;
    using Newtonsoft.Json;
    using Objects.Composition.Graph;

    public static class GraphContextExtensions
    {
        public static void FromJson(this GraphContext graphContext, string json)
        {
            var specification = JsonConvert.DeserializeObject<GraphTemplate>(json);

            graphContext.Container.FromJson(json);

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
