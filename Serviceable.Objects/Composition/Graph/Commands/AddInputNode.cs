namespace Serviceable.Objects.Composition.Graph.Commands
{
    using System;
    using Exceptions;

    public sealed class AddInputNode : ICommand<ContextGraph, ContextGraph>
    {
        private readonly Type type;
        private readonly string id;

        public AddInputNode(Type type, string id)
        {
            this.type = type;
            this.id = id;
        }

        public ContextGraph Execute(ContextGraph context)
        {
            Check.ArgumentNull(type, nameof(type));

            var abstractContext = context.Container.Resolve(type) as AbstractContext;
            Check.ArgumentNull(abstractContext, nameof(type), "Type should be derived from Context");

            var node = new ContextGraphNode(abstractContext, context, id);
            context.InputNodes.Add(node);
            context.Nodes.Add(node);

            return context;
        }
    }
}
