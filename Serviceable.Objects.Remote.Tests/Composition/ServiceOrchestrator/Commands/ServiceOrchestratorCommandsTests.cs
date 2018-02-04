namespace Serviceable.Objects.Remote.Tests.Composition.ServiceOrchestrator.Commands
{
    using System.Collections.Generic;
    using Objects.Composition.ServiceOrchestrator;
    using Objects.Dependencies;
    using Remote.Composition.ServiceOrchestrator.Commands;
    using Remote.Composition.ServiceOrchestrator.Commands.Data;
    using Xunit;

    public sealed class ServiceOrchestratorCommandsTests
    {
        [Fact]
        public void SetBinding_WhenSettingANonExistingKey_ThenItSuccessfullySetsIt()
        {
            var serviceOrchestratorStub = new ServiceOrchestratorStub();

            var commandToTest = new SetBinding(new BindingData
            {
                ServiceName = "service",
                InBindings = new List<InBinding>
                {
                    new InBinding
                    {
                        ContextTypeName = "context",
                        ScaleSetBindings = new List<Binding>
                        {
                            new Binding
                            {
                                ["key"] = "value"
                            }
                        }
                    }
                }
            });

            commandToTest.Execute(serviceOrchestratorStub);

            Assert.NotNull(serviceOrchestratorStub.InBindingsPerService);
            Assert.NotNull(serviceOrchestratorStub.InBindingsPerService["service"]);
        }

        [Fact]
        public void SetBinding_WhenSettingAExistingKey_ThenItSuccessfullySetsIt()
        {
            var serviceOrchestratorStub = new ServiceOrchestratorStub();
            serviceOrchestratorStub.InBindingsPerService.Add("service", new List<InBinding>());

            var commandToTest = new SetBinding(new BindingData
            {
                ServiceName = "service",
                InBindings = new List<InBinding>
                {
                    new InBinding
                    {
                        ContextTypeName = "context",
                        ScaleSetBindings = new List<Binding>
                        {
                            new Binding
                            {
                                ["key"] = "value"
                            }
                        }
                    }
                }
            });

            commandToTest.Execute(serviceOrchestratorStub);

            Assert.NotNull(serviceOrchestratorStub.InBindingsPerService);
            Assert.NotNull(serviceOrchestratorStub.InBindingsPerService["service"]);
        }

        public class ServiceOrchestratorStub : IServiceOrchestrator
        {
            public IList<ServiceRegistration> ServiceRegistrations { get; }
            public string OrchestratorName { get; }
            public string EntryAssemblyFullPath { get; }
            public bool UseChildProcesses { get; }
            public Container ServiceOrchestratorContainer { get; }
            public IDictionary<string, string> GraphTemplatesDictionary { get; }
            public IDictionary<string, List<InBinding>> InBindingsPerService { get; } = new Dictionary<string, List<InBinding>>();
            public IDictionary<string, List<ExternalBinding>> ExternalBindingsPerService { get; } = new Dictionary<string, List<ExternalBinding>>();
        }
    }
}
