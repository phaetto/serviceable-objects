namespace Serviceable.Objects.Windows
{
    using System.ServiceProcess;
    using Composition.Service;
    using Host;
    using Remote.Composition.Host.Commands;

    public class ServiceHostWindowsService : ServiceBase
    {
        private readonly IService service;
        private readonly string[] consoleArgs;
        private ServiceHost serviceHost;

        public ServiceHostWindowsService(string[] consoleArgs)
        {
            this.consoleArgs = consoleArgs;
        }

        public ServiceHostWindowsService(IService service)
        {
            this.service = service;
        }

        protected override void OnStart(string[] args)
        {
            serviceHost = service != null ? new ServiceHost(service) : new ServiceHost(consoleArgs ?? args);

            serviceHost.ForceExecuteAsync(new RunAndBlock());
        }

        protected override void OnStop()
        {
            serviceHost.Execute(new CloseHost());
        }
    }
}