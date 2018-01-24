namespace Serviceable.Objects.Windows.Installation
{
    using System.Collections;
    using System.IO;
    using Newtonsoft.Json;
    using Remote;

    public sealed class UninstallAsServiceCommand : ReproducibleCommandWithData<InstallationContext, InstallationContext, InstallAsServiceData>
    {
        public UninstallAsServiceCommand(InstallAsServiceData data)
            : base(data)
        {
        }

        public override InstallationContext Execute(InstallationContext context)
        {
            using (var installer = new InstallAsServiceCommand(Data).GetInstaller())
            {
                var state = JsonConvert.DeserializeObject<Hashtable>(File.ReadAllText(InstallAsServiceCommand.StateFileName), InstallAsServiceCommand.InstallationStateJsonSerializerSettings);
                try
                {
                    installer.Uninstall(state);
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
    }
}
