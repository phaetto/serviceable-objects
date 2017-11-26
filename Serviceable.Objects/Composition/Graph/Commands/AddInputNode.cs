namespace Serviceable.Objects.Composition.Graph.Commands
{
    using System;
    using Exceptions;

    public sealed class AddInputNode : ICommand<GraphContext, GraphContext>
    {
        private readonly Type type;
        private readonly string id;

        public AddInputNode(Type type, string id)
        {
            this.type = type;
            this.id = id;
        }

        public GraphContext Execute(GraphContext graphContext)
        {
            Check.ArgumentNull(type, nameof(type));

            var abstractContext = graphContext.Container.Resolve(type) as AbstractContext;
            Check.ArgumentNull(abstractContext, nameof(type), "Type should be derived from Context");

            var node = new GraphNodeContext(abstractContext, graphContext, id);
            graphContext.InputNodes.Add(node);
            graphContext.Nodes.Add(node);

            return graphContext;
        }
    }
}
