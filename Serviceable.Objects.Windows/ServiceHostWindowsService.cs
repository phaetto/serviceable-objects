namespace Serviceable.Objects.Windows
{
    using System.ServiceProcess;
    using Composition.Graph;
    using Host;
    using Remote.Composition.Host.Commands;

    public class ServiceHostWindowsService : ServiceBase
    {
        private readonly GraphContext graphContext;
        private readonly string[] consoleArgs;
        private ServiceHost serviceHost;

        public ServiceHostWindowsService(string[] consoleArgs)
        {
            this.consoleArgs = consoleArgs;
        }

        public ServiceHostWindowsService(GraphContext graphContext)
        {
            this.graphContext = graphContext;
        }

        protected override void OnStart(string[] args)
        {
            if (graphContext != null)
            {
                serviceHost = new ServiceHost(graphContext);
            }
            else
            {
                serviceHost = new ServiceHost(consoleArgs ?? args);
            }

            serviceHost.ForceExecuteAsync(new RunAndBlock());
        }

        protected override void OnStop()
        {
            serviceHost.Execute(new CloseHost());
        }
    }
}