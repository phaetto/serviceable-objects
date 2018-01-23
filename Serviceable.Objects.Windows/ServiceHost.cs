namespace Serviceable.Objects.Windows
{
    using System;
    using System.Linq;
    using Composition.Graph;
    using Composition.Service;
    using Composition.ServiceOrchestrator;
    using Installation;
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
            if (ServiceOrchestrator == null)
            {
                return;
            }

            var instrumentationId = GraphContext.GetNodeIds<InstrumentationServerContext>().FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(instrumentationId))
            {
                return;
            }

            GraphContext.AddNode(typeof(InstrumentationServerContext), DefaultInstrumentationId);
        }

        public static bool InstallIfRequested(string []args, InstallAsServiceData installAsServiceData)
        {
            if (args.Length != 1)
            {
                return false;
            }

            switch (args[0])
            {
                case "--install":
                    new InstallationContext().Execute(new InstallAsServiceCommand(installAsServiceData));
                    Console.WriteLine($"Service '{installAsServiceData.Name}' has been installed.");
                    return true;
                case "--uninstall":
                    new InstallationContext().Execute(new InstallAsServiceCommand(installAsServiceData));
                    Console.WriteLine($"Service '{installAsServiceData.Name}' has been uninstalled.");
                    return true;
            }

            return false;
        }
    }
}
