namespace Serviceable.Objects.Windows.Installation
{
    using System.ServiceProcess;

    public class InstallAsServiceData
    {
        public string Name;
        public string Description;
        public ServiceAccount Account;
        public bool DelayAutoStart;
    }
}
