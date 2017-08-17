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
        private readonly List<ContextGraphNode> rootNodes = new List<ContextGraphNode>(); // TODO: rename to input nodes
        internal readonly List<ContextGraphNode> AllNodes = new List<ContextGraphNode>(); // TODO: needs to have proper implementation with vertices and nodes

        public ContextGraph(Container container = null)
        {
            this.container = container ?? new Container();
        }

        public void AddRoot(Type type, string id)
        {
            Check.ArgumentNull(type, nameof(type));

            var abstractContext = container.Resolve(type) as AbstractContext;
            Check.ArgumentNull(abstractContext, nameof(type), "Type should be derived from Context");

            var node = new ContextGraphNode(abstractContext, this, id);
            rootNodes.Add(node);
            AllNodes.Add(node);
        }

        public void AddNode(Type type, string id, string parentNodeUniqueId)
        {
            Check.ArgumentNull(type, nameof(type));
            Check.ArgumentNull(parentNodeUniqueId, nameof(parentNodeUniqueId));

            var parentNode = AllNodes.FirstOrDefault(x => x.UniqueId == parentNodeUniqueId);
            Check.ArgumentNull(parentNode, nameof(parentNodeUniqueId), $"Parent node with id '${parentNodeUniqueId}' could not be found");

            var abstractContext = container.Resolve(type) as AbstractContext;
            Check.ArgumentNull(abstractContext, nameof(type), "Type should be derived from Context");

            var node = new ContextGraphNode(abstractContext, this, id, parentNode);
            AllNodes.Add(node);
        }

        public void AddNode(string fromId, string toId)
        {
            Check.ArgumentNull(fromId, nameof(fromId));
            Check.ArgumentNull(toId, nameof(toId));

            var parentNode = AllNodes.FirstOrDefault(x => x.UniqueId == fromId);
            Check.ArgumentNull(parentNode, nameof(fromId), $"Parent node with id '${fromId}' could not be found");

            var childNode = AllNodes.FirstOrDefault(x => x.UniqueId == toId);
            Check.ArgumentNull(childNode, nameof(toId), $"Parent node with id '${toId}' could not be found");

            var node = new ContextGraphNode(childNode.HostedContextAsAbstractContext, this, fromId, parentNode);
            AllNodes.Add(node);
        }

        public IEnumerable<Stack<EventResult>> Execute(dynamic command)
        {
            var allResultStacks = new List<Stack<EventResult>>(rootNodes.Count);
            var oneHasRun = false;
            foreach (var rootNode in rootNodes)
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
                return rootNodes.First(x => x.UniqueId == uniqueId).Execute(command);
            }
            catch (RuntimeBinderException ex)
            {
                throw new NotSupportedException(
                    "This type of command is not supported by context (Tip: Only one implementation of ICommand<,> can be inferred automatically)",
                    ex);
            }
        }
    }
}
