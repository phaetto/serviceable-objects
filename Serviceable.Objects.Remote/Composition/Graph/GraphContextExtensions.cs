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
            var graphTemplate = JsonConvert.DeserializeObject<GraphTemplate>(json);

            From(graphContext, graphTemplate);
        }

        public static void From(this GraphContext graphContext, GraphTemplate graphTemplate)
        {
            foreach (var graphNode in graphTemplate.GraphNodes)
            {
                if (graphTemplate.GraphVertices.All(x => x.ToId != graphNode.Id))
                {
                    graphContext.AddInput(Types.FindType(graphNode.TypeFullName), graphNode.Id);
                }
                else
                {
                    graphContext.AddNode(Types.FindType(graphNode.TypeFullName), graphNode.Id);
                }
            }

            foreach (var graphVertex in graphTemplate.GraphVertices)
            {
                graphContext.ConnectNodes(graphVertex.FromId, graphVertex.ToId);
            }
        }
    }
}
