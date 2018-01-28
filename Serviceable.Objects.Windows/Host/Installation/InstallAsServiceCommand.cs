namespace Serviceable.Objects.Windows.Host.Installation
{
    using System.Collections;
    using System.Configuration.Install;
    using System.IO;
    using System.Reflection;
    using System.ServiceProcess;
    using Microsoft.Win32;
    using Newtonsoft.Json;
    using Remote;

    public sealed class InstallAsServiceCommand : ReproducibleCommandWithData<InstallationContext, InstallationContext, InstallAsServiceData>
    {
        internal const string StateFileName = "installation-state.json";
        internal static readonly JsonSerializerSettings InstallationStateJsonSerializerSettings = new JsonSerializerSettings
        { 
            TypeNameHandling = TypeNameHandling.All
        };

        public InstallAsServiceCommand(InstallAsServiceData data)
            : base(data)
        {
        }

        public override InstallationContext Execute(InstallationContext context)
        {
            using (var installer = GetInstaller())
            {
                IDictionary state = new Hashtable();
                try
                {
                    installer.Install(state);
                    installer.Commit(state);

                    File.WriteAllText(StateFileName, JsonConvert.SerializeObject(state, InstallationStateJsonSerializerSettings));
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

        internal ServiceInstaller GetInstaller()
        {
            var serviceInstaller = new ServiceInstaller();
            var serviceProcessInstaller = new ServiceProcessInstaller();
            serviceInstaller.ServiceName = Data.Name;
            serviceInstaller.Description = Data.Description;
            serviceProcessInstaller.Account = Data.Account;
            serviceProcessInstaller.Installers.Add(serviceInstaller);

            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.DelayedAutoStart = Data.DelayAutoStart;
            serviceInstaller.Context = new InstallContext();
            serviceInstaller.Context.Parameters["assemblypath"] = $"{Assembly.GetEntryAssembly().Location}";

            serviceInstaller.AfterInstall += ServiceInstaller_AfterInstall;

            return serviceInstaller;
        }

        private void ServiceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Data.ApplicationHostDataConfigurationAsJsonForCommandLine))
            {
                return;
            }

            var system = Registry.LocalMachine.OpenSubKey("System");
            var currentControlSet = system?.OpenSubKey("CurrentControlSet");
            var servicesKey = currentControlSet?.OpenSubKey("Services");
            var serviceKey = servicesKey?.OpenSubKey(Data.Name, true);
            serviceKey?.SetValue("ImagePath",
                $"{(string) serviceKey.GetValue("ImagePath")} \"{Data.ApplicationHostDataConfigurationAsJsonForCommandLine}\"");
        }
    }
}
