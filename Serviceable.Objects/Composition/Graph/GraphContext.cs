namespace Serviceable.Objects.Composition.Graph
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Commands.Node;
    using Commands.NodeInstance;
    using Commands.NodeInstance.ExecutionData;
    using Dependencies;
    using Exceptions;
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
            Nodes.ForEach(x => x.Execute(new ConfigureNode()));
        }

        public void Setup()
        {
            Nodes.ToList().ForEach(x => x.Execute(new SetupNode()));
        }

        public void Initialize()
        {
            Nodes.ForEach(x => x.Execute(new InitializeNode()));
        }

        public void ConfigureNode(string nodeId)
        {
            Check.ArgumentNullOrWhiteSpace(nodeId, nameof(nodeId));
            Nodes.First(x => x.Id == nodeId).Execute(new ConfigureNode());
        }

        public void InitializeNode(string nodeId)
        {
            Check.ArgumentNullOrWhiteSpace(nodeId, nameof(nodeId));
            Nodes.First(x => x.Id == nodeId).Execute(new InitializeNode());
        }

        public GraphNodeContext GetNodeById(string nodeId)
        {
            Check.ArgumentNullOrWhiteSpace(nodeId, nameof(nodeId));
            return Nodes.First(x => x.Id == nodeId);
        }

        public List<ExecutionCommandResult> Execute(dynamic command)
        {
            Check.ArgumentNull(command, nameof(command));

            var contextExecutionResults = new List<ExecutionCommandResult>(InputNodes.Count);

            foreach (var inputNode in InputNodes)
            {
                contextExecutionResults.Add(inputNode.ExecuteGraphCommand(command));
            }

            if (contextExecutionResults.All(x => x.IsIdle))
            {
                throw new NotSupportedException("No context found that support this command");
            }

            if (contextExecutionResults.Any(x => x.IsFaulted))
            {
                throw new AggregateException("Errors while running command", contextExecutionResults.Select(x => x.Exception));
            }

            return contextExecutionResults;
        }

        public ExecutionCommandResult Execute(dynamic command, string uniqueId)
        {
            Check.ArgumentNull(command, nameof(command));
            Check.ArgumentNullOrWhiteSpace(uniqueId, nameof(uniqueId));

            // TODO: error reporting?

            return InputNodes.First(x => x.Id == uniqueId).ExecuteGraphCommand(command);
        }

        public IEnumerable<GraphNodeContext> GetChildren(string id)
        {
            Check.ArgumentNullOrWhiteSpace(id, nameof(id));

            return Vertices.Where(x => x.FromId == id).Select(x => Nodes.First(y => y.Id == x.ToId));
        }
    }
}
