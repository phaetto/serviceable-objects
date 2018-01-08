namespace Serviceable.Objects.Remote.Composition.ServiceOrchestrator.Commands
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using Data;
    using Dependencies;
    using Graph;
    using Host.Configuration;
    using Newtonsoft.Json;
    using Service.Configuration;

    public class NewService : ReproducibleCommandWithData<ServiceOrchestratorContext, ServiceOrchestratorContext, NewServiceData>
    {
        private const string DotNetCoreExecutable = "dotnet.exe";
        private readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public NewService(NewServiceData data) : base(data)
        {
        }

        public override ServiceOrchestratorContext Execute(ServiceOrchestratorContext context)
        {
            if (!context.GraphTemplatesDictionary.ContainsKey(Data.ServiceName))
            {
                throw new InvalidOperationException($"Service {Data.ServiceName} does not exists");
            }

            var template = context.GraphTemplatesDictionary[Data.ServiceName];
            var existingProcess = Process.GetCurrentProcess();
            var executableFile = Path.GetFileName(existingProcess.MainModule.FileName);

            var applicationHostDataConfiguration = new ApplicationHostDataConfiguration
            {
                ServiceGraphTemplate = JsonConvert.DeserializeObject<GraphTemplate>(template), // TODO: optimize the deserialization
                DependencyInjectionRegistrationTemplate = JsonConvert.DeserializeObject<DependencyInjectionRegistrationTemplate>(template),
                ServiceContextConfiguration = new ServiceContextConfiguration
                {
                    OrchestratorName = context.OrchestratorName,
                    ServiceName = Data.ServiceName,
                    TemplateName = Data.ServiceName,
                    ExternalBindings = context.ExternalBindingsPerService?[Data.ServiceName],
                    InBindings = context.InBindingsPerService?[Data.ServiceName],
                },
            };

            var applicationHostDataConfigurationAsJsonForCommandLine =
                JsonConvert.SerializeObject(applicationHostDataConfiguration, jsonSerializerSettings)
                .Replace('\"', '\'');

            Process serviceProcess;
            if (executableFile == DotNetCoreExecutable)
            {
                if (string.IsNullOrWhiteSpace(context.EntryAssemblyFullPath))
                {
                    throw new InvalidOperationException("EntryAssemblyFullPath should be defined in configuration for dotnet core apps");
                }

                serviceProcess = new Process
                {
                    StartInfo =
                    {
                        UseShellExecute = false,
                        FileName = existingProcess.MainModule.FileName, // This is dotnet running (.NET core)
                        CreateNoWindow = true,
                        Arguments = $"\"{context.EntryAssemblyFullPath}\" \"{applicationHostDataConfigurationAsJsonForCommandLine}\"",
                    }
                };
            }
            else
            {
                serviceProcess = new Process
                {
                    StartInfo =
                    {
                        UseShellExecute = false,
                        FileName = existingProcess.MainModule.FileName, // This is custom executable wrapping host (.NET Framework)
                        CreateNoWindow = true,
                        Arguments = $"\"{applicationHostDataConfigurationAsJsonForCommandLine}\"",
                    }
                };
            }

            serviceProcess.Start();

            return context;
        }
    }
}