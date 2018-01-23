namespace Serviceable.Objects.Windows
{
    using System.Linq;
    using Composition.Graph;
    using Composition.Service;
    using Composition.ServiceOrchestrator;
    using Instrumentation.Server;
    using Remote.Composition.Host;

    public class ServiceHost : ApplicationHost
    {
        private const string DefaultInstrumentationId = "service-instrumentation-server";

        public ServiceHost(IServiceOrchestrator serviceOrchestrator, GraphContext graphContext) : base(serviceOrchestrator, graphContext)
        {
            CheckAndSetupInstrumentationServer();
        }

        public ServiceHost(IService service) : base(service)
        {
        }

        public ServiceHost(string[] args) : base(args)
        {
        }

        public ServiceHost(string jsonString) : base(jsonString)
        {
            CheckAndSetupInstrumentationServer();
        }

        private void CheckAndSetupInstrumentationServer()
        {
            if (ServiceOrchestrator != null)
            {
                var instrumentationId = GraphContext.GetNodeIds<InstrumentationServerContext>().FirstOrDefault();
                if (string.IsNullOrWhiteSpace(instrumentationId))
                {
                    GraphContext.AddNode(typeof(InstrumentationServerContext), DefaultInstrumentationId);
                    // Advance new nodes to Config -> Setup -> Init
                    GraphContext.ConfigureSetupAndInitialize();
                }
            }
        }
    }
}
