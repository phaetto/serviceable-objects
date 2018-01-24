namespace Serviceable.Objects.Windows.Host.Installation
{
    using System.ServiceProcess;

    public class InstallAsServiceData
    {
        public string Name;
        public string Description;
        public ServiceAccount Account;
        public bool DelayAutoStart;
        public string ApplicationHostDataConfigurationAsJsonForCommandLine;
    }
}
