namespace Serviceable.Objects.Composition.Graph
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Commands.Node;
    using Commands.NodeInstance.ExecutionData;
    using Dependencies;
    using Exceptions;
    using Service;
    using Stages.Configuration;

    public sealed class GraphContext : Context<GraphContext> // TODO: IDisposable
    {
        public readonly Container Container;
        internal readonly List<GraphNodeContext> InputNodes = new List<GraphNodeContext>(); // TODO: fields can be made private
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

            var node = new GraphNodeContext(type, this, id);
            InputNodes.Add(node);
            Nodes.Add(node);
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

            var node = new GraphNodeContext(type, this, id);
            Nodes.Add(node);
        }

        public void AddNode(AbstractContext context, string id)
        {
            Check.ArgumentNull(context, nameof(context));
            Check.ArgumentNullOrWhiteSpace(id, nameof(id));

            var node = new GraphNodeContext(context, this, id);
            Nodes.Add(node);
        }

        public void RemoveNode(string id)
        {
            Check.ArgumentNullOrWhiteSpace(id, nameof(id));

            var node = Nodes.First(x => x.Id == id);
            Nodes.Remove(node);
            InputNodes.Remove(node);
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
                ToId = toId
            });
        }

        public void DisconnectNodes(string fromId, string toId)
        {
            Check.ArgumentNullOrWhiteSpace(fromId, nameof(fromId));
            Check.ArgumentNullOrWhiteSpace(toId, nameof(toId));

            var parentNode = Nodes.FirstOrDefault(x => x.Id == fromId);
            Check.ArgumentNull(parentNode, nameof(fromId), $"Parent node with id '${fromId}' could not be found");

            var childNode = Nodes.FirstOrDefault(x => x.Id == toId);
            Check.ArgumentNull(childNode, nameof(toId), $"Parent node with id '${toId}' could not be found");

            var vertix = Vertices.FirstOrDefault(x => x.FromId == fromId && x.ToId == toId);
            Check.Argument(vertix == null, nameof(fromId), "Vertex doesn't exist in this graph");

            Vertices.Remove(vertix);
        }

        public void DisconnectNode(string id)
        {
            Check.ArgumentNullOrWhiteSpace(id, nameof(id));

            var parentNode = Nodes.FirstOrDefault(x => x.Id == id);
            Check.ArgumentNull(parentNode, nameof(id), $"Parent node with id '${id}' could not be found");

            var vertices = Vertices.Where(x => x.FromId == id || x.ToId == id).ToList();
            foreach (var vertix in vertices)
            {
                Vertices.Remove(vertix);
            }
        }

        public IEnumerable<string> GetNodeIds<TNodeInContext>()
        {
            return Nodes.Where(x => x.ContextType == typeof(TNodeInContext)).Select(x => x.Id);
        }

        public void ConfigureSetupAndInitialize() 
        {
            // Configure/Setup/Initialize - ordering matters
            var service = Container.Resolve<IService>(throwOnError: false);
            var configurationSource = Container.Resolve<IConfigurationSource>(throwOnError: false);
            Nodes.ForEach(x => x.Execute(new ConfigureNode(service, configurationSource)));
            Nodes.ToList().ForEach(x => x.Execute(new SetupNode()));
            Nodes.Where(x => !x.IsConfigured).ToList().ForEach(x => x.Execute(new ConfigureNode(service, configurationSource)));
            Nodes.ForEach(x => x.Execute(new InitializeNode()));
        }

        public void UninitializeDismantleAndDeconfigure() 
        {
            // Uninitialize/Dismantle/Deconfigure - ordering matters
            Nodes.ForEach(x => x.Execute(new DeinitializeNode()));
            Nodes.ToList().ForEach(x => x.Execute(new DismantleNode()));
            Nodes.ForEach(x => x.Execute(new DeconfigureNode()));
        }

        public GraphNodeContext GetNodeById(string nodeId)
        {
            Check.ArgumentNullOrWhiteSpace(nodeId, nameof(nodeId));
            return Nodes.First(x => x.Id == nodeId);
        }

        public List<ExecutionCommandResult> Execute(object command)
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

        public ExecutionCommandResult Execute(object command, string uniqueId)
        {
            Check.ArgumentNull(command, nameof(command));
            Check.ArgumentNullOrWhiteSpace(uniqueId, nameof(uniqueId));

            var contextExecutionResult = InputNodes.First(x => x.Id == uniqueId).ExecuteGraphCommand(command);

            if (contextExecutionResult.IsIdle)
            {
                throw new NotSupportedException("No context found that support this command");
            }

            if (contextExecutionResult.IsFaulted)
            {
                throw new AggregateException("Errors while running command", contextExecutionResult.Exception);
            }

            return contextExecutionResult;
        }

        public IEnumerable<GraphNodeContext> GetChildren(string id)
        {
            Check.ArgumentNullOrWhiteSpace(id, nameof(id));

            return Vertices.Where(x => x.FromId == id).Select(x => Nodes.First(y => y.Id == x.ToId));
        }
    }
}
