namespace Serviceable.Objects.Remote.Composition.ServiceOrchestrator.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using Data;
    using Dependencies;
    using Events;
    using Graph;
    using Host.Configuration;
    using Newtonsoft.Json;
    using Objects.Composition.ServiceOrchestrator;
    using Service.Configuration;

    public class NewService : ReproducibleCommandWithData<IServiceOrchestrator, IServiceOrchestrator, NewServiceData>, IEventProducer
    {
        private const string DotNetCoreExecutable = "dotnet.exe";
        private readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };
        public List<IEvent> EventsProduced { get; } = new List<IEvent>();

        public NewService(NewServiceData data) : base(data)
        {
        }

        public override IServiceOrchestrator Execute(IServiceOrchestrator context)
        {
            if (!context.GraphTemplatesDictionary.ContainsKey(Data.ServiceName))
            {
                throw new InvalidOperationException($"Service {Data.ServiceName} does not exists");
            }

            var template = context.GraphTemplatesDictionary[Data.ServiceName];
            var inBindings = context.InBindingsPerService == null
                ? new List<InBinding>()
                : context.InBindingsPerService.ContainsKey(Data.ServiceName)
                    ? context.InBindingsPerService[Data.ServiceName]
                    : new List<InBinding>();
            var externalBindings = context.ExternalBindingsPerService == null
                ? new List<ExternalBinding>()
                : context.ExternalBindingsPerService.ContainsKey(Data.ServiceName)
                    ? context.ExternalBindingsPerService[Data.ServiceName]
                    : new List<ExternalBinding>();

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
                    ExternalBindings = externalBindings,
                    InBindings = inBindings
                }
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
                        CreateNoWindow = !context.UseChildProcesses,
                        Arguments = $"\"{context.EntryAssemblyFullPath}\" \"{applicationHostDataConfigurationAsJsonForCommandLine}\""
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
                        CreateNoWindow = !context.UseChildProcesses,
                        Arguments = $"\"{applicationHostDataConfigurationAsJsonForCommandLine}\""
                    }
                };
            }

            serviceProcess.Start();

            // TODO: how to make sure that the process started without instrumentation?

            EventsProduced.Add(new ServiceStarted {ProcessId = serviceProcess.Id, ServiceName = Data.ServiceName});

            return context;
        }
    }
}