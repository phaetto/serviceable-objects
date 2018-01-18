namespace Serviceable.Objects.Tests
{
    using System;
    using System.Collections.Generic;
    using Composition.Graph;
    using Composition.Graph.Commands.Node;
    using Composition.Graph.Commands.NodeInstance.ExecutionData;
    using Composition.Graph.Stages.Configuration;
    using Composition.Service;
    using Composition.ServiceOrchestrator;
    using Dependencies;
    using Newtonsoft.Json;
    using Xunit;

    public sealed class GraphNodeContextTests
    {
        [Fact]
        public void Execute_WhenSuccesfullyExecutingAnInstance_ThenTheResultObjectContainsTheValue()
        {
            var command = new TestCommand();
            var graphContext = new GraphContext();
            graphContext.AddNode(typeof(TestContext), "test-node");
            var graphNodeContext = graphContext.GetNodeById("test-node");

            // Set up service
            graphContext.Container.RegisterWithDefaultInterface(new TestService());
            graphContext.Container.RegisterWithDefaultInterface(new TestConfigurationSource());

            // Configure
            graphContext.ConfigureSetupAndInitialize();
            Assert.True(graphNodeContext.IsConfigured);

            // Execute
            var result = graphNodeContext.ExecuteGraphCommand(command);

            Assert.NotNull(result);
            Assert.False(result.IsFaulted);
            Assert.False(result.IsIdle);
            Assert.Null(result.Exception);
            Assert.NotNull(result.SingleContextExecutionResultWithInfo);
            Assert.Equal(typeof(TestContext), result.SingleContextExecutionResultWithInfo.ContextType);
            Assert.Equal("test-node", result.SingleContextExecutionResultWithInfo.NodeId);
            Assert.Equal("success", result.SingleContextExecutionResultWithInfo.ResultObject);
        }

        public struct TestContextConfiguration
        {
            public string CustomValue { get; set; }
            public string InValue { get; set; }
            public string OutValue { get; set; }
        }

        public class TestContext : Context<TestContext>, IConfigurableStageFactory
        {
            public TestContextConfiguration TestContextConfiguration { get; set; }
            public bool HasBeenConfigured { get; set; }
            public object GenerateConfigurationCommand(string serializedConfigurationString)
            {
                return new TestConfigurationCommand(serializedConfigurationString);
            }
        }

        public class TestConfigurationSource : IConfigurationSource
        {
            public string GetConfigurationValueForKey(string serviceName, string graphNodeId, string typeName)
            {
                return JsonConvert.SerializeObject(new TestContextConfiguration
                {
                    CustomValue = "$in.Custom",
                    InValue = "$in.Host",
                    OutValue = "$out.Host"
                });
            }
        }

        public class TestCommand : ICommand<TestContext, string>
        {
            public string Execute(TestContext context)
            {
                return "success";
            }
        }

        public class TestConfigurationCommand : ICommand<TestContext, TestContext>
        {
            private readonly string serializedConfigurationString;

            public TestConfigurationCommand(string serializedConfigurationString)
            {
                this.serializedConfigurationString = serializedConfigurationString;
            }

            public TestContext Execute(TestContext context)
            {
                context.HasBeenConfigured = true;
                context.TestContextConfiguration =
                    JsonConvert.DeserializeObject<TestContextConfiguration>(serializedConfigurationString);

                Assert.Equal("custom-value", context.TestContextConfiguration.CustomValue);

                return context;
            }
        }

        public class TestService : IService
        {
            public string OrchestratorName { get; }
            public string ServiceName { get; } = "service-x";
            public string TemplateName { get; }
            public IList<InBinding> InBindings { get; } = new List<InBinding>
            {
                new InBinding
                {
                    ContextTypeName = typeof(TestContext).AssemblyQualifiedName,
                    ScaleSetBindings = new List<Binding>
                    {
                        new Binding
                        {
                            Host = "in-host-1",
                            ["Custom"] = "custom-value"
                        },
                        new Binding
                        {
                            Host = "in-host-2",
                            ["Custom"] = "custom-value"
                        }
                    }
                }
            };
            public IList<ExternalBinding> ExternalBindings { get; } = new List<ExternalBinding>
            {
                new ExternalBinding
                {
                    ContextTypeName = typeof(TestContext).AssemblyQualifiedName,
                    AlgorithmBindings = new List<AlgorithmBinding>
                    {
                        new AlgorithmBinding
                        {
                            AlgorithmTypeName = typeof(TestAlgorithm).AssemblyQualifiedName,
                            ScaleSetBindings = new List<Binding>
                            {
                                new Binding
                                {
                                    Host = "out-host-1"
                                },
                                new Binding
                                {
                                    Host = "out-host-2"
                                }
                            }
                        }
                    }
                }
            };
            public Container ServiceContainer { get; }
            public GraphContext GraphContext { get; }
        }

        public class TestAlgorithm : IAlgorithmicInstanceExecution
        {
            public IList<GraphNodeInstanceContext> FindGraphNodeInstanceContextsToBeExecuted(List<GraphNodeInstanceContext> graphNodeInstanceContexts)
            {
                Assert.Equal(4, graphNodeInstanceContexts.Count);
                return graphNodeInstanceContexts;
            }

            public IList<GraphNodeInstanceContext> ContinueExecutionGraphNodeInstanceContextsToBeExecuted(IList<ExecutionCommandResult> previousExecutionCommandResults,
                List<GraphNodeInstanceContext> graphNodeInstanceContexts)
            {
                throw new NotImplementedException();
            }

            public IList<ExecutionCommandResult> FilterExecutionResults(IList<ExecutionCommandResult> previousExecutionCommandResults, IList<ExecutionCommandResult> currentExecutionCommandResults)
            {
                throw new NotImplementedException();
            }
        }
    }
}
