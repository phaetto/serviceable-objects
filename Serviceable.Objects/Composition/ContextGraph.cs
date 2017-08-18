namespace Serviceable.Objects.Composition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CSharp.RuntimeBinder;
    using Serviceable.Objects.Dependencies;
    using Serviceable.Objects.Exceptions;

    public sealed class ContextGraph : Context<ContextGraph>
    {
        internal readonly Container Container;
        internal readonly List<ContextGraphNode> InputNodes = new List<ContextGraphNode>();
        internal readonly List<ContextGraphVertex> Vertices = new List<ContextGraphVertex>();
        internal readonly List<ContextGraphNode> Nodes = new List<ContextGraphNode>();

        public ContextGraph(Container container = null)
        {
            this.Container = container ?? new Container();
        }

        public void AddInput(Type type, string id)
        {
            Check.ArgumentNull(type, nameof(type));
            Check.ArgumentNullOrWhiteSpace(id, nameof(id));

            var abstractContext = Container.CreateObject(type) as AbstractContext;
            Check.ArgumentNull(abstractContext, nameof(type), "Type should be derived from Context");

            AddInput(abstractContext, id);
        }

        public void AddInput(AbstractContext context, string id)
        {
            Check.ArgumentNull(context, nameof(context));
            Check.ArgumentNullOrWhiteSpace(id, nameof(id));

            var node = new ContextGraphNode(context, this, id);
            InputNodes.Add(node);
            Nodes.Add(node);
        }

        public void AddNode(Type type, string id)
        {
            Check.ArgumentNull(type, nameof(type));
            Check.ArgumentNullOrWhiteSpace(id, nameof(id));

            var abstractContext = Container.CreateObject(type) as AbstractContext;
            Check.ArgumentNull(abstractContext, nameof(type), "Type should be derived from Context");

            AddNode(abstractContext, id);
        }

        public void AddNode(AbstractContext context, string id)
        {
            Check.ArgumentNull(context, nameof(context));
            Check.ArgumentNullOrWhiteSpace(id, nameof(id));

            var node = new ContextGraphNode(context, this, id);
            Nodes.Add(node);
        }

        public void ConnectNodes(string fromId, string toId)
        {
            Check.ArgumentNullOrWhiteSpace(fromId, nameof(fromId));
            Check.ArgumentNullOrWhiteSpace(toId, nameof(toId));

            var parentNode = Nodes.FirstOrDefault(x => x.Id == fromId);
            Check.ArgumentNull(parentNode, nameof(fromId), $"Parent node with id '${fromId}' could not be found");

            var childNode = Nodes.FirstOrDefault(x => x.Id == toId);
            Check.ArgumentNull(childNode, nameof(toId), $"Parent node with id '${toId}' could not be found");

            Check.Argument(Vertices.Any(x => x.FromId == fromId && x.ToId == toId), nameof(fromId), "Vertex already exists in this graph");

            Vertices.Add(new ContextGraphVertex
            {
                FromId = fromId,
                ToId = toId,
            });
        }

        public IEnumerable<Stack<EventResult>> Execute(dynamic command)
        {
            Check.ArgumentNull(command, nameof(command));

            var allResultStacks = new List<Stack<EventResult>>(InputNodes.Count);
            var oneHasRun = false;
            foreach (var inputNode in InputNodes)
            {
                try
                {
                    var resultExecutionStack = new Stack<EventResult>();
                    inputNode.Execute(command, resultExecutionStack);
                    allResultStacks.Add(resultExecutionStack);
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
            Check.ArgumentNull(command, nameof(command));
            Check.ArgumentNullOrWhiteSpace(uniqueId, nameof(uniqueId));

            try
            {
                var resultExecutionStack = new Stack<EventResult>();
                InputNodes.First(x => x.Id == uniqueId).Execute(command, resultExecutionStack);
                return resultExecutionStack;
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
            Check.ArgumentNullOrWhiteSpace(id, nameof(id));

            return Vertices.Where(x => x.FromId == id).Select(x => Nodes.First(y => y.Id == x.ToId));
        }
    }
}
