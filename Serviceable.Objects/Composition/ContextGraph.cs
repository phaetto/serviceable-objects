namespace Serviceable.Objects.Composition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CSharp.RuntimeBinder;
    using Serviceable.Objects.Dependencies;
    using Serviceable.Objects.Exceptions;

    public sealed class ContextGraph
    {
        private readonly Container container;
        private readonly List<ContextGraphNode> inputNodes = new List<ContextGraphNode>();
        private readonly List<ContextGraphVertex> vertices = new List<ContextGraphVertex>();
        private readonly List<ContextGraphNode> nodes = new List<ContextGraphNode>();

        public readonly Stack<EventResult> ResultsExecutionStack = new Stack<EventResult>();

        public ContextGraph(Container container = null)
        {
            this.container = container ?? new Container();
        }

        public void AddInput(Type type, string id)
        {
            Check.ArgumentNull(type, nameof(type));

            var abstractContext = container.Resolve(type) as AbstractContext;
            Check.ArgumentNull(abstractContext, nameof(type), "Type should be derived from Context");

            var node = new ContextGraphNode(abstractContext, this, id);
            inputNodes.Add(node);
            nodes.Add(node);
        }

        public void AddNode(Type type, string id)
        {
            Check.ArgumentNull(type, nameof(type));

            var abstractContext = container.Resolve(type) as AbstractContext;
            Check.ArgumentNull(abstractContext, nameof(type), "Type should be derived from Context");

            var node = new ContextGraphNode(abstractContext, this, id);
            nodes.Add(node);
        }

        public void ConnectNodes(string fromId, string toId)
        {
            Check.ArgumentNull(fromId, nameof(fromId));
            Check.ArgumentNull(toId, nameof(toId));

            var parentNode = nodes.FirstOrDefault(x => x.Id == fromId);
            Check.ArgumentNull(parentNode, nameof(fromId), $"Parent node with id '${fromId}' could not be found");

            var childNode = nodes.FirstOrDefault(x => x.Id == toId);
            Check.ArgumentNull(childNode, nameof(toId), $"Parent node with id '${toId}' could not be found");

            Check.Argument(vertices.Any(x => x.FromId == fromId && x.ToId == toId), nameof(fromId), "Vertex already exists in this graph");

            vertices.Add(new ContextGraphVertex
            {
                FromId = fromId,
                ToId = toId,
            });
        }

        public IEnumerable<Stack<EventResult>> Execute(dynamic command)
        {
            var allResultStacks = new List<Stack<EventResult>>(inputNodes.Count);
            var oneHasRun = false;
            foreach (var rootNode in inputNodes)
            {
                try
                {
                    allResultStacks.Add(rootNode.Execute(command));
                    oneHasRun = true;
                }
                catch (RuntimeBinderException)
                {
                    // Only apply to the ones that support that
                }
            }

            if (!oneHasRun)
            {
                throw new NotSupportedException("No context found that support this command");
            }

            return allResultStacks;
        }

        public Stack<EventResult> Execute(dynamic command, string uniqueId)
        {
            try
            {
                return inputNodes.First(x => x.Id == uniqueId).Execute(command);
            }
            catch (RuntimeBinderException ex)
            {
                throw new NotSupportedException(
                    "This type of command is not supported by context (Tip: Only one implementation of ICommand<,> can be inferred automatically)",
                    ex);
            }
        }

        public IEnumerable<ContextGraphNode> GetChildren(string id)
        {
            return vertices.Where(x => x.FromId == id).Select(x => nodes.First(y => y.Id == x.ToId));
        }
    }
}
