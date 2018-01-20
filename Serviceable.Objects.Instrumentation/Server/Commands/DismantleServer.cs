namespace Serviceable.Objects.Instrumentation.Server.Commands
{
    using Objects.Composition.Graph;
    using Objects.Composition.Graph.Commands.Node;

    public sealed class DismantleServer : ICommand<InstrumentationServerContext, InstrumentationServerContext>
    {
        private readonly GraphContext graphContext;
        private readonly GraphNodeContext graphNodeContext;

        public DismantleServer(GraphContext graphContext, GraphNodeContext graphNodeContext)
        {
            this.graphContext = graphContext;
            this.graphNodeContext = graphNodeContext;
        }

        public InstrumentationServerContext Execute(InstrumentationServerContext context)
        {
            var namedPipeServerNodeId = $"{graphNodeContext.Id}-namepipe-server";

            // Remove the setup nodes

            var namedPipeServerContextNode = graphContext.GetNodeById(namedPipeServerNodeId);

            // Deconfigure
            namedPipeServerContextNode.ExecuteGraphCommand(new DeconfigureNode()); // Deconfigure

            // Diconnect graph nodes
            graphContext.DisconnectNode(namedPipeServerNodeId);
            graphContext.RemoveNode(namedPipeServerNodeId);

            return context;
        }

        public static string WellknownPipeFormat(string serviceOrchestratorName, string serviceName)
        {
            var suffix = string.IsNullOrWhiteSpace(serviceName) 
                ? $"{serviceOrchestratorName}/orchestrator" 
                : $"{serviceOrchestratorName}/service/{serviceName}";

            return $"serviceable-objects/instrumentation/{suffix}";
        }
    }
}