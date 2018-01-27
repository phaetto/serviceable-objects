namespace Serviceable.Objects.Windows.Host
{
    using System;
    using System.Linq;
    using Composition.Graph;
    using Composition.Service;
    using Installation;
    using Instrumentation.Server;
    using Remote.Composition.Host;

    public class ServiceHost : ApplicationHost
    {
        private const string DefaultInstrumentationId = "service-instrumentation-server";

        public ServiceHost(GraphContext graphContext) : base(graphContext)
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
            var instrumentationId = GraphContext.GetNodeIds<InstrumentationServerContext>().FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(instrumentationId))
            {
                return;
            }

            GraphContext.AddNode(typeof(InstrumentationServerContext), DefaultInstrumentationId);
        }

        public static bool ChangeInstallStateIfRequested(string[] args, InstallAsServiceData installAsServiceData)
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
                    new InstallationContext().Execute(new UninstallAsServiceCommand(installAsServiceData));
                    Console.WriteLine($"Service '{installAsServiceData.Name}' has been uninstalled.");
                    return true;
            }

            return false;
        }
    }
}
