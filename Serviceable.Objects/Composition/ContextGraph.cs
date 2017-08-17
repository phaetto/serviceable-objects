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
        private readonly List<ContextGraphNode> rootNodes = new List<ContextGraphNode>();
        internal readonly List<ContextGraphNode> AllNodes = new List<ContextGraphNode>();

        public ContextGraph(Container container = null)
        {
            this.container = container ?? new Container();
        }

        public Guid AddRoot(Type type)
        {
            Check.ArgumentNull(type, nameof(type));

            var abstractContext = container.Resolve(type) as AbstractContext;
            Check.ArgumentNull(abstractContext, nameof(type), "Type should be derived from Context");

            var node = new ContextGraphNode(abstractContext, this);
            rootNodes.Add(node);
            AllNodes.Add(node);
            return node.UniqueId;
        }

        public Guid AddNode(Type type, Guid parentNodeUniqueId)
        {
            Check.ArgumentNull(type, nameof(type));
            Check.ArgumentNull(parentNodeUniqueId, nameof(parentNodeUniqueId));

            var rootNode = AllNodes.FirstOrDefault(x => x.UniqueId == parentNodeUniqueId);
            Check.ArgumentNull(rootNode, nameof(parentNodeUniqueId), "Parent node could not be found");

            var abstractContext = container.Resolve(type) as AbstractContext;
            Check.ArgumentNull(abstractContext, nameof(type), "Type should be derived from Context");

            var node = new ContextGraphNode(abstractContext, this, rootNode);
            AllNodes.Add(node);
            return node.UniqueId;
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

        public Stack<EventResult> Execute(dynamic command, Guid uniqueId)
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
