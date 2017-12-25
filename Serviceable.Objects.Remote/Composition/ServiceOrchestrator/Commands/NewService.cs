namespace Serviceable.Objects.Remote.Composition.ServiceOrchestrator.Commands
{
    using System;
    using System.Diagnostics;
    using Data;
    using Dependencies;
    using Graph;
    using Host.Configuration;
    using Newtonsoft.Json;
    using Service.Configuration;

    public class NewService : ReproducibleCommandWithData<ServiceOrchestratorContext, ServiceOrchestratorContext, NewServiceData>
    {
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

            // TODO: check for dotnet.exe

            // TODO: add the entry dll to config (orchestrator)
            var entryAssembly =
                "C:\\sources\\serviceable-objects\\Examples\\TestHttpCompositionConsoleApp\\bin\\Debug\\netcoreapp1.0\\TestHttpCompositionConsoleApp.dll";

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

            var serviceProcess = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    FileName = existingProcess.MainModule.FileName, // This is dotnet running
                    CreateNoWindow = true,
                    Arguments = $"\"{entryAssembly}\" \"" + applicationHostDataConfigurationAsJsonForCommandLine + "\"",
                }
            };
            var started = serviceProcess.Start();

            return context;
        }
    }
}