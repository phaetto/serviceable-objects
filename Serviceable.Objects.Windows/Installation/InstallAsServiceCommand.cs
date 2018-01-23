namespace Serviceable.Objects.Windows.Installation
{
    using System.Collections;
    using System.Configuration.Install;
    using System.Reflection;
    using System.ServiceProcess;
    using Remote;

    public sealed class InstallAsServiceCommand : ReproducibleCommandWithData<InstallationContext, InstallationContext, InstallAsServiceData>
    {
        public InstallAsServiceCommand(InstallAsServiceData data)
            : base(data)
        {
        }

        public override InstallationContext Execute(InstallationContext context)
        {
            using (var installer = GetInstaller(Data))
            {
                IDictionary state = new Hashtable();
                try
                {
                    installer.Install(state);
                    installer.Commit(state);
                }
                catch
                {
                    try
                    {
                        installer.Rollback(state);
                    }
                    catch
                    {
                        // Try to rollback on error, but do not record if something wrong happened
                    }

                    throw;
                }
            }

            return context;
        }

        internal static ServiceInstaller GetInstaller(InstallAsServiceData data)
        {
            var serviceInstaller = new ServiceInstaller();
            var serviceProcessInstaller = new ServiceProcessInstaller();
            serviceInstaller.ServiceName = data.Name;
            serviceInstaller.Description = data.Description;
            serviceProcessInstaller.Account = data.Account;
            serviceProcessInstaller.Installers.Add(serviceInstaller);

            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.DelayedAutoStart = data.DelayAutoStart;
            serviceInstaller.Context = new InstallContext();
            serviceInstaller.Context.Parameters["assemblypath"] = Assembly.GetEntryAssembly().Location;

            return serviceInstaller;
        }
    }
}
