namespace Serviceable.Objects.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Composition.Graph;
    using Composition.Graph.Commands.NodeInstance;
    using Microsoft.CSharp.RuntimeBinder;
    using Xunit;

    public sealed class GraphNodeInstanceContextTests
    {
        [Fact]
        public void Execute_WhenSuccesfullyExecutingAnInstance_ThenTheResultObjectContainsTheValue()
        {
            var testContext = new TestContext();
            var command = new SuccessFullCommand();
            var graphContext = new GraphContext();
            var graphNodeContext = new GraphNodeContext(testContext, graphContext, "test-node");
            var graphNodeInstanceContext = new GraphNodeInstanceContext(testContext, graphContext, graphNodeContext, "test-node");

            var result = graphNodeInstanceContext.Execute(new ExecuteCommand(command));

            Assert.NotNull(result);
            Assert.False(result.IsFaulted);
            Assert.False(result.IsIdle);
            Assert.Null(result.Exception);
            Assert.NotNull(result.SingleContextExecutionResultWithInfo);
            Assert.Equal(typeof(TestContext), result.SingleContextExecutionResultWithInfo.ContextType);
            Assert.Equal("test-node", result.SingleContextExecutionResultWithInfo.NodeId);
            Assert.Equal("success", result.SingleContextExecutionResultWithInfo.ResultObject);
        }

        [Fact]
        public void Execute_WhenExecutingAnInstanceAndAnErrorHappens_ThenTheResultObjectContainsTheErrorDetails()
        {
            var testContext = new TestContext();
            var command = new FailedCommand();
            var graphContext = new GraphContext();
            var graphNodeContext = new GraphNodeContext(testContext, graphContext, "test-node");
            var graphNodeInstanceContext = new GraphNodeInstanceContext(testContext, graphContext, graphNodeContext, "test-node");

            var result = graphNodeInstanceContext.Execute(new ExecuteCommand(command));

            Assert.NotNull(result);
            Assert.True(result.IsFaulted);
            Assert.False(result.IsIdle);
            Assert.NotNull(result.Exception);
            Assert.Null(result.SingleContextExecutionResultWithInfo);
        }

        [Fact]
        public void Execute_WhenExecutingAnInstanceAndDoesNotSupportCommand_ThenItIsConsideredIdle()
        {
            var testContext = new TestContext();
            var command = new CommandFromAnotherContext();
            var graphContext = new GraphContext();
            var graphNodeContext = new GraphNodeContext(testContext, graphContext, "test-node");
            var graphNodeInstanceContext = new GraphNodeInstanceContext(testContext, graphContext, graphNodeContext, "test-node");

            var result = graphNodeInstanceContext.Execute(new ExecuteCommand(command));

            Assert.NotNull(result);
            Assert.False(result.IsFaulted);
            Assert.True(result.IsIdle);
            Assert.NotNull(result.Exception);
            Assert.IsType<RuntimeBinderException>(result.Exception);
            Assert.Null(result.SingleContextExecutionResultWithInfo);
        }

        // TODO: add interface tests
        // IGraphFlowEventPushControlEvent, IPostGraphFlowPullControl

        public class TestContext : Context<TestContext>
        {
        }

        public class AnotherTestContext : Context<AnotherTestContext>
        {
        }

        public class SuccessFullCommand : ICommand<TestContext, string>
        {
            public string Execute(TestContext context)
            {
                return "success";
            }
        }

        public class FailedCommand : ICommand<TestContext, string>
        {
            public string Execute(TestContext context)
            {
                throw new Exception("error");
            }
        }

        public class CommandFromAnotherContext : ICommand<AnotherTestContext, string>
        {
            public string Execute(AnotherTestContext context)
            {
                return "success";
            }
        }

        [Fact]
        public void ProcessNodeInstanceEventLogic_WhenAnEventIsPublishedAndNotSupported_ThenExecutionReturnsAnEmptyList()
        {
            var testContext = new TestContext();
            var graphContext = new GraphContext();
            var graphNodeContext = new GraphNodeContext(testContext, graphContext, "test-node");
            var graphNodeInstanceContext = new GraphNodeInstanceContext(testContext, graphContext, graphNodeContext, "test-node");

            var result = graphNodeInstanceContext.Execute(new ProcessNodeInstanceEventLogic(new EventNotSupportedForTestContext(), graphNodeInstanceContext)).ToList();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        public class EventNotSupportedForTestContext : IEvent
        {
        }

        [Fact]
        public void Execute_WhenExecutingAnAsyncGenericTaskReturnCommand_ThenItIsCorrectlyWaitingForIt()
        {
            var testContext = new TestContext();
            var command = new SuccessFullAsyncCommand();
            var graphContext = new GraphContext();
            var graphNodeContext = new GraphNodeContext(testContext, graphContext, "test-node");
            var graphNodeInstanceContext = new GraphNodeInstanceContext(testContext, graphContext, graphNodeContext, "test-node");

            var result = graphNodeInstanceContext.Execute(new ExecuteCommand(command));

            Assert.NotNull(result);
            Assert.False(result.IsFaulted);
            Assert.False(result.IsIdle);
            Assert.Null(result.Exception);
            Assert.NotNull(result.SingleContextExecutionResultWithInfo);
            Assert.Equal(typeof(TestContext), result.SingleContextExecutionResultWithInfo.ContextType);
            Assert.Equal("test-node", result.SingleContextExecutionResultWithInfo.NodeId);
            Assert.Equal("success", result.SingleContextExecutionResultWithInfo.ResultObject);
        }

        public class SuccessFullAsyncCommand : ICommand<TestContext, Task<string>>
        {
            public async Task<string> Execute(TestContext context)
            {
                await Task.Delay(10);
                return "success";
            }
        }

        [Fact]
        public void Execute_WhenExecutingAnAsyncReturnTaskCommand_ThenItIsCorrectlyWaitingForIt()
        {
            var testContext = new TestContext();
            var command = new SuccessFullAsyncNoResultCommand();
            var graphContext = new GraphContext();
            var graphNodeContext = new GraphNodeContext(testContext, graphContext, "test-node");
            var graphNodeInstanceContext = new GraphNodeInstanceContext(testContext, graphContext, graphNodeContext, "test-node");

            var result = graphNodeInstanceContext.Execute(new ExecuteCommand(command));

            Assert.NotNull(result);
            Assert.False(result.IsFaulted);
            Assert.False(result.IsIdle);
            Assert.Null(result.Exception);
            Assert.NotNull(result.SingleContextExecutionResultWithInfo);
            Assert.Equal(typeof(TestContext), result.SingleContextExecutionResultWithInfo.ContextType);
            Assert.Equal("test-node", result.SingleContextExecutionResultWithInfo.NodeId);
            Assert.Null(result.SingleContextExecutionResultWithInfo.ResultObject);
        }

        public class SuccessFullAsyncNoResultCommand : ICommand<TestContext, Task>
        {
            public async Task Execute(TestContext context)
            {
                await Task.Delay(10);
            }
        }
    }
}
