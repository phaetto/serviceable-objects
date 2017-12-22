namespace Serviceable.Objects.Tests
{
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
            graphContext.ConfigureNode("test-node");

            Assert.True(graphNodeContext.IsConfigured);

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
            public string InValue { get; set; }
            public string OutValue { get; set; }
        }

        public class TestContext : Context<TestContext>, IConfigurableStageFactory
        {
            public TestContextConfiguration TestContextConfiguration { get; set; }
            public bool HasBeenConfigured { get; set; }
            public dynamic GenerateConfigurationCommand(string serializedConfigurationString)
            {
                return new TestConfigurationCommand(serializedConfigurationString);
            }
        }

        public class TestConfigurationSource : IConfigurationSource
        {
            public string GetConfigurationValueForKey(string serviceName, string graphNodeId, string typeName)
            {
                return JsonConvert.SerializeObject(new TestContextConfiguration { InValue = "$in.host", OutValue = "$out.host"});
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
                            Host = "in-host-1"
                        },
                        new Binding
                        {
                            Host = "in-host-2"
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
                throw new System.NotImplementedException();
            }

            public IList<ExecutionCommandResult> FilterExecutionResults(IList<ExecutionCommandResult> previousExecutionCommandResults, IList<ExecutionCommandResult> currentExecutionCommandResults)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
