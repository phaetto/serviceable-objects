namespace Serviceable.Objects.Remote.Composition.ServiceOrchestrator
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Dependencies;
    using Graph;
    using Host.Configuration;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Objects.Composition.ServiceOrchestrator;
    using Service.Configuration;

    public sealed class ContainerPreparationManagementService
    {
        private const string DotNetCoreExecutable = "dotnet.exe";
        private readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };
        private readonly IServiceOrchestrator context;
        private readonly string templateName;
        private readonly string serviceName;

        public ContainerPreparationManagementService(
            IServiceOrchestrator context,
            string templateName,
            string serviceName)
        {
            this.context = context;
            this.templateName = templateName;
            this.serviceName = serviceName;
        }

        public ProcessStartInfo PrepareStartInfoForProcess()
        {
            var applicationHostDataConfiguration = PrepareApplicationHostDataConfiguration();

            var applicationHostDataConfigurationAsJsonForCommandLine =
                JsonConvert.SerializeObject(applicationHostDataConfiguration, jsonSerializerSettings)
                .Replace('\"', '\'');

            var existingProcess = Process.GetCurrentProcess();
            var executableFile = Path.GetFileName(existingProcess.MainModule.FileName);

            var startInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                FileName = existingProcess.MainModule.FileName, // This is could be dotnet running (under .NET core) instead of a host process
                CreateNoWindow = !context.UseChildProcesses,
            };

            if (executableFile == DotNetCoreExecutable)
            {
                if (string.IsNullOrWhiteSpace(context.EntryAssemblyFullPath))
                {
                    throw new InvalidOperationException("EntryAssemblyFullPath should be defined in configuration for dotnet core apps");
                }

                startInfo.Arguments = $"\"{context.EntryAssemblyFullPath}\" \"{applicationHostDataConfigurationAsJsonForCommandLine}\"";
            }
            else
            {
                startInfo.Arguments = $"\"{applicationHostDataConfigurationAsJsonForCommandLine}\"";
            }

            return startInfo;
        }

        public ApplicationHostDataConfiguration PrepareApplicationHostDataConfiguration()
        {
            if (!context.GraphTemplatesDictionary.ContainsKey(serviceName) && string.IsNullOrWhiteSpace(templateName))
            {
                throw new InvalidOperationException($"Service {serviceName} does not exists");
            }

            if (!string.IsNullOrWhiteSpace(templateName) && !context.GraphTemplatesDictionary.ContainsKey(templateName))
            {
                throw new InvalidOperationException($"Template {templateName} does not exists");
            }

            var template = GetServiceTemplate();
            var inBindings = GetServiceInBindings();
            var externalBindings = GetServiceExternalBindings();

            var applicationHostDataConfiguration = new ApplicationHostDataConfiguration
            {
                ServiceGraphTemplate =
                    JsonConvert.DeserializeObject<GraphTemplate>(template), // TODO: optimize the deserialization
                DependencyInjectionRegistrationTemplate =
                    JsonConvert.DeserializeObject<DependencyInjectionRegistrationTemplate>(template),
                ServiceContextConfiguration = new ServiceContextConfiguration
                {
                    OrchestratorName = context.OrchestratorName,
                    ServiceName = serviceName,
                    TemplateName = serviceName,
                    ExternalBindings = externalBindings,
                    InBindings = inBindings
                }
            };
            return applicationHostDataConfiguration;
        }

        private string GetServiceTemplate()
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

        private List<InBinding> GetServiceInBindings()
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

        private List<ExternalBinding> GetServiceExternalBindings()
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