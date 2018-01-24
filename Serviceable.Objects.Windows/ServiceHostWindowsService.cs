namespace Serviceable.Objects.Windows
{
    using System.ServiceProcess;
    using Host;
    using Remote.Composition.Host.Commands;

    public class ServiceHostWindowsService : ServiceBase
    {
        private readonly string[] consoleArgs;
        private ServiceHost serviceHost;

        public ServiceHostWindowsService(string[] consoleArgs)
        {
            this.consoleArgs = consoleArgs;
        }

        protected override void OnStart(string[] args)
        {
            serviceHost = new ServiceHost(consoleArgs ?? args);
            serviceHost.ForceExecuteAsync(new RunAndBlock());
        }

        protected override void OnStop()
        {
            serviceHost.Execute(new CloseHost());
        }
    }
}