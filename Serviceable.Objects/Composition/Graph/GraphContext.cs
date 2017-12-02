namespace Serviceable.Objects.Composition.Graph
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Dependencies;
    using Exceptions;
    using Microsoft.CSharp.RuntimeBinder;
    using Stages.Configuration;

    public sealed class GraphContext : Context<GraphContext> // TODO: IDisposable
    {
        public readonly Container Container;
        internal readonly List<GraphNodeContext> InputNodes = new List<GraphNodeContext>();
        internal readonly List<GraphVertexContext> Vertices = new List<GraphVertexContext>();
        internal readonly List<GraphNodeContext> Nodes = new List<GraphNodeContext>();

        public GraphContext(Container container = null)
        {
            Container = container ?? new Container();
            Container.Register(this);
        }

        // TODO: break public methods to commands

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

            var node = new GraphNodeContext(context, this, id);
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

            var node = new GraphNodeContext(context, this, id);
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

            Vertices.Add(new GraphVertexContext
            {
                FromId = fromId,
                ToId = toId,
            });
        }

        public IEnumerable<string> GetNodeIds<TNodeInContext>()
        {
            return Nodes.Where(x => x.HostedContext is TNodeInContext).Select(x => x.Id);
        }

        public void Configure() // TODO: Configure/Setup/Initialize - create a workflow for those steps (ordering matters)
        {
            var configurationSource = Container.Resolve<IConfigurationSource>(throwOnError: false);
            if (configurationSource != null)
            {
                Nodes.ForEach(x => x.Configure(configurationSource));
            }
        }

        public void Setup()
        {
            Nodes.ToList().ForEach(x => x.Setup());
        }

        public void Initialize()
        {
            Nodes.ForEach(x => x.Initialize());
        }

        public void ConfigureNode(string nodeId)
        {
            Check.ArgumentNullOrWhiteSpace(nodeId, nameof(nodeId));
            var configurationSource = Container.Resolve<IConfigurationSource>();
            Nodes.First(x => x.Id == nodeId).Configure(configurationSource);
        }

        public void InitializeNode(string nodeId)
        {
            Check.ArgumentNullOrWhiteSpace(nodeId, nameof(nodeId));
            Nodes.First(x => x.Id == nodeId).Initialize();
        }

        public GraphNodeContext GetNodeById(string nodeId)
        {
            Check.ArgumentNullOrWhiteSpace(nodeId, nameof(nodeId));
            return Nodes.First(x => x.Id == nodeId);
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
                catch (NotSupportedException)
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

        public IEnumerable<GraphNodeContext> GetChildren(string id)
        {
            Check.ArgumentNullOrWhiteSpace(id, nameof(id));

            return Vertices.Where(x => x.FromId == id).Select(x => Nodes.First(y => y.Id == x.ToId));
        }
    }
}
