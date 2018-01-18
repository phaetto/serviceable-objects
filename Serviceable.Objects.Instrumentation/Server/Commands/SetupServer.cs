namespace Serviceable.Objects.Instrumentation.Server.Commands
{
    using System;
    using System.Linq;
    using IO.NamedPipes.Server;
    using IO.NamedPipes.Server.Configuration;
    using Objects.Composition.Graph;
    using Objects.Composition.Service;
    using Objects.Composition.ServiceOrchestrator;

    public sealed class SetupServer : ICommand<InstrumentationServerContext, InstrumentationServerContext>
    {
        private readonly GraphContext graphContext;
        private readonly GraphNodeContext graphNodeContext;

        public SetupServer(GraphContext graphContext, GraphNodeContext graphNodeContext)
        {
            this.graphContext = graphContext;
            this.graphNodeContext = graphNodeContext;
        }

        public InstrumentationServerContext Execute(InstrumentationServerContext context)
        {
            var service = graphContext.Container.Resolve<IService>(throwOnError: false);
            var serviceOrchestrator = graphContext.Container.Resolve<IServiceOrchestrator>(throwOnError: false);

            var anotherInstrumentationContext = graphContext.GetNodeIds<InstrumentationServerContext>();
            if (anotherInstrumentationContext.Count() > 1)
            {
                throw new InvalidOperationException("Only once instance of InstrumentationServerContext per service is allowed.");
            }

            var namedPipeServerNodeId = $"{graphNodeContext.Id}-namepipe-server";

            // Add one node, the server/listener - no input nodes.

            graphContext.AddNode(new NamedPipeServerContext(new NamedPipeServerConfiguration
            {
                PipeName = WellknownPipeFormat(serviceOrchestrator?.OrchestratorName ?? service.OrchestratorName, service?.ServiceName)
            }), namedPipeServerNodeId);

            // Connect to this node
            graphContext.ConnectNodes(namedPipeServerNodeId, graphNodeContext.Id);

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