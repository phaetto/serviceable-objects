namespace Serviceable.Objects.Tests
{
    using System;
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
        public void Execute_WhenExecutingAnInstanceAndDoesNotSupportCommand_ThenTheItIsConsideredIdle()
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
    }
}
