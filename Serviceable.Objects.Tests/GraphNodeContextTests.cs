﻿namespace Serviceable.Objects.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Composition.Graph;
    using Composition.Graph.Commands.Node;
    using Composition.Graph.Commands.NodeInstance.ExecutionData;
    using Composition.Graph.Events;
    using Composition.Graph.Stages.Configuration;
    using Composition.Graph.Stages.Initialization;
    using Composition.Service;
    using Composition.ServiceOrchestrator;
    using Dependencies;
    using Exceptions;
    using Newtonsoft.Json;
    using Xunit;

    public sealed class GraphNodeContextTests
    {
        [Fact]
        public void Execute_WhenSuccesfullyExecutingAnInstance_ThenTheResultObjectContainsTheValue()
        {
            var command = new TestCommand();
            var graphContext = new GraphContext();
            graphContext.AddNode(new TestContext(), "test-node");

            // Set up service
            graphContext.Container.RegisterWithDefaultInterface(new TestService());
            graphContext.Container.RegisterWithDefaultInterface(new TestConfigurationSource());

            // Configure
            graphContext.ConfigureSetupAndInitialize();

            // Execute
            var result = graphContext.Execute(command, "test-node");

            Assert.NotNull(result);
            Assert.False(result.IsFaulted);
            Assert.False(result.IsIdle);
            Assert.False(result.IsPaused);
            Assert.Null(result.Exception);
            Assert.NotNull(result.SingleContextExecutionResultWithInfo);
            Assert.Equal(typeof(TestContext), result.SingleContextExecutionResultWithInfo.ContextType);
            Assert.Equal("test-node", result.SingleContextExecutionResultWithInfo.NodeId);
            Assert.Equal("success", result.SingleContextExecutionResultWithInfo.ResultObject);
        }

        [Fact]
        public void Execute_WhenSuccesfullyExecutingAnInstanceFromContainer_ThenTheResultObjectContainsTheValue()
        {
            var command = new TestCommand();
            var graphContext = new GraphContext();
            graphContext.AddNode(typeof(TestContext), "test-node");

            // Set up service
            graphContext.Container.RegisterWithDefaultInterface(new TestService());
            graphContext.Container.RegisterWithDefaultInterface(new TestConfigurationSource());

            // Configure
            graphContext.ConfigureSetupAndInitialize();

            // Execute
            var result = graphContext.Execute(command, "test-node");

            Assert.NotNull(result);
            Assert.False(result.IsFaulted);
            Assert.False(result.IsIdle);
            Assert.False(result.IsPaused);
            Assert.Null(result.Exception);
            Assert.NotNull(result.SingleContextExecutionResultWithInfo);
            Assert.Equal(typeof(TestContext), result.SingleContextExecutionResultWithInfo.ContextType);
            Assert.Equal("test-node", result.SingleContextExecutionResultWithInfo.NodeId);
            Assert.Equal("success", result.SingleContextExecutionResultWithInfo.ResultObject);
        }

        [Fact]
        public void Execute_WhenSuccesfullyUnloadingAGraph_ThenTheGraphIsEmpty()
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
            Assert.True(graphNodeContext.Status == GraphNodeStatus.Initialized);

            // Execute
            var result = graphContext.Execute(command, "test-node");
            Assert.NotNull(result);
            Assert.False(result.IsFaulted);
            Assert.False(result.IsIdle);
            Assert.False(result.IsPaused);
            Assert.Null(result.Exception);

            // Unload
            graphContext.UninitializeDismantleAndDeconfigure();
            Assert.False(graphNodeContext.Status == GraphNodeStatus.Initialized);
        }

        [Fact]
        public void Execute_WhenSuccesfullyLoadingAndUnloadingAGraphMultipleTimes_ThenTheGraphIsStillInitializingCorrectly()
        {
            var command = new TestCommand();
            var graphContext = new GraphContext();
            graphContext.AddNode(typeof(TestContext), "test-node");
            var graphNodeContext = graphContext.GetNodeById("test-node");

            // Set up service
            graphContext.Container.RegisterWithDefaultInterface(new TestService());
            graphContext.Container.RegisterWithDefaultInterface(new TestConfigurationSource());

            // First
            graphContext.ConfigureSetupAndInitialize();
            Assert.True(graphNodeContext.Status == GraphNodeStatus.Initialized);
            graphContext.UninitializeDismantleAndDeconfigure();
            Assert.False(graphNodeContext.Status == GraphNodeStatus.Initialized);

            // Second
            graphContext.ConfigureSetupAndInitialize();
            Assert.True(graphNodeContext.Status == GraphNodeStatus.Initialized);
            graphContext.UninitializeDismantleAndDeconfigure();
            Assert.False(graphNodeContext.Status == GraphNodeStatus.Initialized);

            // Third
            graphContext.ConfigureSetupAndInitialize();
            Assert.True(graphNodeContext.Status == GraphNodeStatus.Initialized);
            graphContext.UninitializeDismantleAndDeconfigure();
            Assert.False(graphNodeContext.Status == GraphNodeStatus.Initialized);

            // Load and Execute
            graphContext.ConfigureSetupAndInitialize();
            Assert.True(graphNodeContext.Status == GraphNodeStatus.Initialized);
            var result = graphContext.Execute(command, "test-node");
            Assert.NotNull(result);
            Assert.False(result.IsFaulted);
            Assert.False(result.IsIdle);
            Assert.False(result.IsPaused);
            Assert.Null(result.Exception);
        }

        [Fact]
        public void Execute_WhenGraphIsPaused_ThenThrowsException()
        {
            var command = new TestCommand();
            var graphContext = new GraphContext();
            graphContext.AddNode(typeof(TestContext), "test-node");

            // Set up service
            graphContext.Container.RegisterWithDefaultInterface(new TestService());
            graphContext.Container.RegisterWithDefaultInterface(new TestConfigurationSource());

            // Configure
            graphContext.ConfigureSetupAndInitialize();
            graphContext.Pause();

            // Execute
            var result = graphContext.Execute(command, "test-node");

            Assert.NotNull(result);
            Assert.False(result.IsFaulted);
            Assert.False(result.IsIdle);
            Assert.True(result.IsPaused);
            Assert.IsType<RuntimeExecutionPausedException>(result.Exception);
            Assert.Null(result.SingleContextExecutionResultWithInfo);
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

            public object GenerateDeconfigurationCommand()
            {
                return null;
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

        [Fact]
        public async Task Execute_WhenGraphIsPaused_ThenItShouldFinalizeTheExistingExecutionCycle()
        {
            var switchToPausedEventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
            var command = new TestContextWithPauseCommand(switchToPausedEventWaitHandle);
            var testCommand = new TestContextWithPauseCommand();
            var graphContext = new GraphContext();
            var testContextWithPause = new TestContextWithPause();
            var secondContext = new SecondContext();
            graphContext.AddNode(testContextWithPause, "first-node");
            graphContext.AddNode(secondContext, "second-node");
            graphContext.ConnectNodes("first-node", "second-node");

            // Configure
            graphContext.ConfigureSetupAndInitialize();

            // Execute (context will pause)
            ExecutionCommandResult executionCommandResult = null;
            Task task = Task.Run(() => executionCommandResult = graphContext.Execute(command, "first-node"));

            // Graph switches to pause
            switchToPausedEventWaitHandle.WaitOne(1000);
            Assert.True(graphContext.IsWorking);
            graphContext.Pause(); // Will wait until all executions that have a read-lock to finish
            Assert.False(graphContext.IsWorking); // In this current setup, nothing else will run here

            // An event-based command should not be executed as well
            testContextWithPause.Execute(new TestContextCommand());
            Assert.Equal(1, secondContext.TimesHasRun);

            // A new command trying to execute should fail
            var result = graphContext.Execute(testCommand, "first-node");
            Assert.NotNull(result);
            Assert.False(result.IsFaulted);
            Assert.False(result.IsIdle);
            Assert.True(result.IsPaused);
            Assert.IsType<RuntimeExecutionPausedException>(result.Exception);
            Assert.Null(result.SingleContextExecutionResultWithInfo);

            // As before, a new command trying to execute should fail as well
            result = graphContext.Execute(testCommand, "first-node");
            Assert.NotNull(result);
            Assert.False(result.IsFaulted);
            Assert.False(result.IsIdle);
            Assert.True(result.IsPaused);
            Assert.IsType<RuntimeExecutionPausedException>(result.Exception);
            Assert.Null(result.SingleContextExecutionResultWithInfo);
            Assert.Equal(1, secondContext.TimesHasRun);

            // Wait for it to finish
            await task;

            // Command already running in the background should continue
            Assert.False(graphContext.IsWorking);
            Assert.NotNull(executionCommandResult);
            Assert.False(executionCommandResult.IsFaulted);
            Assert.False(executionCommandResult.IsIdle);
            Assert.False(executionCommandResult.IsPaused);
            Assert.Null(executionCommandResult.Exception);
            Assert.NotNull(executionCommandResult.SingleContextExecutionResultWithInfo);
            Assert.Equal(1, secondContext.TimesHasRun);
        }

        public class TestContextWithPause : Context<TestContextWithPause>, IInitializeStageFactoryWithDeinitSynchronization
        {
            public ReaderWriterLockSlim ReaderWriterLockSlim { get; set; }

            internal IList<EventResult> PublishEvent(IEvent eventToPublish, bool emulateRun)
            {
                // Best practice for "exactly one" synchronization
                try
                {
                    if (ReaderWriterLockSlim == null)
                    {
                        return new List<EventResult>
                        {
                            new EventResult
                            {
                                ResultObject = new RuntimeExecutionPausedException()
                            }
                        }; // Do not publish and async event
                    }

                    ReaderWriterLockSlim.EnterReadLock();

                    if (emulateRun)
                    {
                        Task.WaitAll(Task.Delay(500));
                    }

                    return PublishContextEvent(eventToPublish);
                }
                finally
                {
                    ReaderWriterLockSlim?.ExitReadLock();
                }
            }

            public object GenerateInitializationCommand()
            {
                return null;
            }

            public object GenerateDeinitializationCommand()
            {
                return null;
            }
        }

        public class TestContextWithPauseCommand: ICommand<TestContextWithPause, TestContextWithPause>
        {
            private readonly EventWaitHandle switchToPausedEventWaitHandle;

            public TestContextWithPauseCommand(EventWaitHandle switchToPausedEventWaitHandle)
            {
                this.switchToPausedEventWaitHandle = switchToPausedEventWaitHandle;
            }

            public TestContextWithPauseCommand()
            {
            }

            public TestContextWithPause Execute(TestContextWithPause context)
            {
                switchToPausedEventWaitHandle?.Set();

                context.PublishEvent(
                    new GraphFlowEventPushControlEventApplyCommandInsteadOfEvent(new SecondContextCommand()),
                    true);

                return context;
            }
        }

        public class TestContextCommand: ICommand<TestContextWithPause, TestContextWithPause>
        {
            public TestContextWithPause Execute(TestContextWithPause context)
            {
                context.PublishEvent(
                    new GraphFlowEventPushControlEventApplyCommandInsteadOfEvent(new SecondContextCommand()),
                    false);

                return context;
            }
        }

        public class SecondContext : Context<SecondContext>
        {
            public int TimesHasRun { get; set; }
        }

        public class SecondContextCommand: ICommand<SecondContext, SecondContext>
        {
            public SecondContext Execute(SecondContext context)
            {
                context.TimesHasRun++;
                return context;
            }
        }
    }
}
