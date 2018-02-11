namespace Serviceable.Objects.Remote.Composition.ServiceOrchestrator.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Data;
    using Dependencies;
    using Events;
    using Graph;
    using Host.Configuration;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
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
            if (!context.GraphTemplatesDictionary.ContainsKey(Data.ServiceName) && string.IsNullOrWhiteSpace(Data.TemplateName))
            {
                throw new InvalidOperationException($"Service {Data.ServiceName} does not exists");
            }

            if (!string.IsNullOrWhiteSpace(Data.TemplateName) && !context.GraphTemplatesDictionary.ContainsKey(Data.TemplateName))
            {
                throw new InvalidOperationException($"Template {Data.TemplateName} does not exists");
            }

            var template = GetServiceTemplate(context, Data.TemplateName, Data.ServiceName);
            var inBindings = GetServiceInBindings(context, Data.TemplateName, Data.ServiceName);
            var externalBindings = GetServiceExternalBindings(context, Data.TemplateName, Data.ServiceName);

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

        private static string GetServiceTemplate(IServiceOrchestrator context, string templateName, string serviceName)
        {
            var template = !string.IsNullOrWhiteSpace(templateName) ? context.GraphTemplatesDictionary.ContainsKey(templateName) ? context.GraphTemplatesDictionary[templateName] : null : null;
            var serviceTemplate = !string.IsNullOrWhiteSpace(serviceName) ? context.GraphTemplatesDictionary.ContainsKey(serviceName) ? context.GraphTemplatesDictionary[serviceName] : null : null;

            if (template == null || serviceTemplate == null)
            {
                return serviceTemplate ?? template;
            }

            var templateJObject = JObject.Parse(template);
            var serviceTemplateJObject = JObject.Parse(serviceTemplate);
            templateJObject.Merge(serviceTemplateJObject,
                new JsonMergeSettings
                {
                    MergeNullValueHandling = MergeNullValueHandling.Ignore,
                    MergeArrayHandling = MergeArrayHandling.Concat
                });

            return templateJObject.ToString(Formatting.None);
        }

        private static List<InBinding> GetServiceInBindings(IServiceOrchestrator context, string templateName, string serviceName)
        {
            if (context.InBindingsPerService == null)
            {
                return new List<InBinding>();
            }

            var template = !string.IsNullOrWhiteSpace(templateName) ? context.InBindingsPerService.ContainsKey(templateName) ? context.InBindingsPerService[templateName] : null : null;
            var serviceTemplate = !string.IsNullOrWhiteSpace(serviceName) ? context.InBindingsPerService.ContainsKey(serviceName) ? context.InBindingsPerService[serviceName] : null : null;

            if (template == null || serviceTemplate == null)
            {
                return serviceTemplate ?? template ?? new List<InBinding>();
            }

            template.ForEach(x =>
            {
                foreach (var inBinding in serviceTemplate.Where(y => y.ContextTypeName == x.ContextTypeName))
                {
                    var newList = new List<Binding>();
                    newList.AddRange(x.ScaleSetBindings);
                    newList.AddRange(inBinding.ScaleSetBindings);
                    x.ScaleSetBindings = newList;
                }
            });

            serviceTemplate.ForEach(x =>
            {
                if (template.All(y => y.ContextTypeName != x.ContextTypeName))
                {
                    template.Add(x);
                }
            });

            return template;
        }

        private static List<ExternalBinding> GetServiceExternalBindings(IServiceOrchestrator context, string templateName, string serviceName)
        {
            if (context.ExternalBindingsPerService == null)
            {
                return new List<ExternalBinding>();
            }

            // TODO: refactor
            var template = !string.IsNullOrWhiteSpace(templateName) ? context.ExternalBindingsPerService.ContainsKey(templateName) ? context.ExternalBindingsPerService[templateName] : null : null;
            var serviceTemplate = !string.IsNullOrWhiteSpace(serviceName) ? context.ExternalBindingsPerService.ContainsKey(templateName) ? context.ExternalBindingsPerService[serviceName] : null : null;

            if (template == null || serviceTemplate == null)
            {
                return serviceTemplate ?? template ?? new List<ExternalBinding>();
            }

            template.ForEach(x =>
            {
                // TODO: refactor
                foreach (var externalBinding in serviceTemplate.Where(y => y.ContextTypeName == x.ContextTypeName))
                {
                    foreach (var objAlgorithmBinding in x.AlgorithmBindings)
                    {
                        foreach (var externalBindingAlgorithmBinding in externalBinding.AlgorithmBindings)
                        {
                            if (externalBindingAlgorithmBinding.AlgorithmTypeName == objAlgorithmBinding.AlgorithmTypeName)
                            {
                                var newList = new List<Binding>();
                                newList.AddRange(objAlgorithmBinding.ScaleSetBindings);
                                newList.AddRange(externalBindingAlgorithmBinding.ScaleSetBindings);
                                objAlgorithmBinding.ScaleSetBindings = newList;
                            }
                        }
                    }

                    foreach (var externalBindingAlgorithmBinding in externalBinding.AlgorithmBindings)
                    {
                        if (x.AlgorithmBindings.All(y =>
                            y.AlgorithmTypeName != externalBindingAlgorithmBinding.AlgorithmTypeName))
                        {
                            x.AlgorithmBindings.Add(externalBindingAlgorithmBinding);
                        }
                    }
                }
            });

            serviceTemplate.ForEach(x =>
            {
                if (template.All(y => y.ContextTypeName != x.ContextTypeName))
                {
                    template.Add(x);
                }
            });

            return template;
        }
    }
}