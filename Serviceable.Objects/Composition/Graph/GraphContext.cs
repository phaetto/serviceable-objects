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
        private readonly List<GraphNodeContext> inputNodes = new List<GraphNodeContext>();
        private readonly List<GraphVertexContext> vertices = new List<GraphVertexContext>();
        private readonly List<GraphNodeContext> nodes = new List<GraphNodeContext>();

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
            inputNodes.Add(node);
            nodes.Add(node);
        }

        public void AddInput(AbstractContext context, string id)
        {
            Check.ArgumentNull(context, nameof(context));
            Check.ArgumentNullOrWhiteSpace(id, nameof(id));

            var node = new GraphNodeContext(context, this, id);
            inputNodes.Add(node);
            nodes.Add(node);
        }

        public void AddNode(Type type, string id)
        {
            Check.ArgumentNull(type, nameof(type));
            Check.ArgumentNullOrWhiteSpace(id, nameof(id));

            var node = new GraphNodeContext(type, this, id);
            nodes.Add(node);
        }

        public void AddNode(AbstractContext context, string id)
        {
            Check.ArgumentNull(context, nameof(context));
            Check.ArgumentNullOrWhiteSpace(id, nameof(id));

            var node = new GraphNodeContext(context, this, id);
            nodes.Add(node);
        }

        public void RemoveNode(string id)
        {
            Check.ArgumentNullOrWhiteSpace(id, nameof(id));

            var node = nodes.First(x => x.Id == id);
            nodes.Remove(node);
            inputNodes.Remove(node);
        }

        public void ConnectNodes(string fromId, string toId)
        {
            Check.ArgumentNullOrWhiteSpace(fromId, nameof(fromId));
            Check.ArgumentNullOrWhiteSpace(toId, nameof(toId));

            var parentNode = nodes.FirstOrDefault(x => x.Id == fromId);
            Check.ArgumentNull(parentNode, nameof(fromId), $"Parent node with id '${fromId}' could not be found");

            var childNode = nodes.FirstOrDefault(x => x.Id == toId);
            Check.ArgumentNull(childNode, nameof(toId), $"Parent node with id '${toId}' could not be found");

            Check.Argument(vertices.Any(x => x.FromId == fromId && x.ToId == toId), nameof(fromId), "Vertex already exists in this graph");

            vertices.Add(new GraphVertexContext
            {
                FromId = fromId,
                ToId = toId
            });
        }

        public void DisconnectNodes(string fromId, string toId)
        {
            Check.ArgumentNullOrWhiteSpace(fromId, nameof(fromId));
            Check.ArgumentNullOrWhiteSpace(toId, nameof(toId));

            var parentNode = nodes.FirstOrDefault(x => x.Id == fromId);
            Check.ArgumentNull(parentNode, nameof(fromId), $"Parent node with id '${fromId}' could not be found");

            var childNode = nodes.FirstOrDefault(x => x.Id == toId);
            Check.ArgumentNull(childNode, nameof(toId), $"Parent node with id '${toId}' could not be found");

            var vertix = vertices.FirstOrDefault(x => x.FromId == fromId && x.ToId == toId);
            Check.Argument(vertix == null, nameof(fromId), "Vertex doesn't exist in this graph");

            vertices.Remove(vertix);
        }

        public void DisconnectNode(string id)
        {
            Check.ArgumentNullOrWhiteSpace(id, nameof(id));

            var parentNode = nodes.FirstOrDefault(x => x.Id == id);
            Check.ArgumentNull(parentNode, nameof(id), $"Parent node with id '${id}' could not be found");

            var verticesToRemove = this.vertices.Where(x => x.FromId == id || x.ToId == id).ToList();
            foreach (var vertix in verticesToRemove)
            {
                this.vertices.Remove(vertix);
            }
        }

        public IEnumerable<string> GetNodeIds<TNodeInContext>()
        {
            return nodes.Where(x => x.ContextType == typeof(TNodeInContext)).Select(x => x.Id);
        }

        public void ConfigureSetupAndInitialize() 
        {
            // Configure/Setup/Initialize - ordering matters
            var service = Container.Resolve<IService>(throwOnError: false);
            var configurationSource = Container.Resolve<IConfigurationSource>(throwOnError: false);
            nodes.ForEach(x => x.Execute(new ConfigureNode(service, configurationSource)));
            nodes.ToList().ForEach(x => x.Execute(new SetupNode()));
            nodes.Where(x => !x.IsConfigured).ToList().ForEach(x => x.Execute(new ConfigureNode(service, configurationSource)));
            nodes.ForEach(x => x.Execute(new InitializeNode()));
        }

        public void UninitializeDismantleAndDeconfigure() 
        {
            // Uninitialize/Dismantle/Deconfigure - ordering matters
            nodes.ForEach(x => x.Execute(new DeinitializeNode()));
            nodes.ToList().ForEach(x => x.Execute(new DismantleNode()));
            nodes.ForEach(x => x.Execute(new DeconfigureNode()));
        }

        public GraphNodeContext GetNodeById(string nodeId)
        {
            Check.ArgumentNullOrWhiteSpace(nodeId, nameof(nodeId));
            return nodes.First(x => x.Id == nodeId);
        }

        public List<ExecutionCommandResult> Execute(object command)
        {
            Check.ArgumentNull(command, nameof(command));

            var contextExecutionResults = new List<ExecutionCommandResult>(inputNodes.Count);

            foreach (var inputNode in inputNodes)
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

            var contextExecutionResult = inputNodes.First(x => x.Id == uniqueId).ExecuteGraphCommand(command);

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

            return vertices.Where(x => x.FromId == id).Select(x => nodes.First(y => y.Id == x.ToId));
        }
    }
}
