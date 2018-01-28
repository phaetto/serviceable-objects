namespace Serviceable.Objects.Remote.Tests
{
    using System.Threading.Tasks;
    using Classes;
    using Classes.Proxies;
    using Objects.Composition.Graph;
    using Objects.Tests.Classes;
    using Proxying;
    using Xunit;

    public class ProxyTests
    {
        [Fact]
        public void ProxyContext_WhenAReproducibleExecutes_ThenItCorrectlyProxiesIt()
        {
            var contextForTest = new ContextForTest();
            var proxyContext = new TypeSafeProxyContext(contextForTest);

            var result = proxyContext.Execute(new ReproducibleTestCommand(new ReproducibleTestData { ChangeToValue = "custom" }));

            Assert.NotNull(result);
            Assert.IsType<TypeSafeProxyContext>(result);
            Assert.Equal("custom", contextForTest.ContextVariable);
        }

        [Fact]
        public void ProxyContext_WhenARemotableExecutes_ThenItCorrectlyProxiesIt()
        {
            var contextForTest = new ContextForTest();
            var proxyContext = new TypeSafeProxyContext(contextForTest);

            var result = proxyContext.Execute(new RemotableTestCommand(new ReproducibleTestData { ChangeToValue = "custom" }));

            Assert.NotNull(result);
            Assert.IsType<ReproducibleTestData>(result);
            Assert.Equal("custom", contextForTest.ContextVariable);
        }

        [Fact]
        public async Task ProxyContext_WhenAReproducibleAsyncExecutes_ThenItCorrectlyProxiesIt()
        {
            var contextForTest = new ContextForTest();
            var proxyContext = new TypeSafeProxyContext(contextForTest);

            var result = await proxyContext.Execute(new ReproducibleTestCommandAsync(new ReproducibleTestData { ChangeToValue = "custom" }));

            Assert.NotNull(result);
            Assert.IsType<TypeSafeProxyContext>(result);
            Assert.Equal("custom", contextForTest.ContextVariable);
        }

        [Fact]
        public async Task ProxyContext_WhenARemotableAsyncExecutes_ThenItCorrectlyProxiesIt()
        {
            var contextForTest = new ContextForTest();
            var proxyContext = new TypeSafeProxyContext(contextForTest);

            var result = await proxyContext.Execute(new RemotableTestCommandAsync(new ReproducibleTestData { ChangeToValue = "custom" }));

            Assert.NotNull(result);
            Assert.IsType<ReproducibleTestData>(result);
            Assert.Equal("custom", contextForTest.ContextVariable);
        }

        [Fact]
        public async Task ProxyContext_WhenACombinationExecutes_ThenItCorrectlyProxiesIt()
        {
            var contextForTest = new ContextForTest();
            var proxyContext = new TypeSafeProxyContext(contextForTest);

            var result = await proxyContext
                .Execute(new ReproducibleTestCommand(new ReproducibleTestData { ChangeToValue = "custom 1" }))
                .Execute(new ReproducibleTestCommandAsync(new ReproducibleTestData { ChangeToValue = "custom 2" }))
                .Execute(new ReproducibleTestCommand(new ReproducibleTestData { ChangeToValue = "custom 3" }))
                .Execute(new ReproducibleTestCommandAsync(new ReproducibleTestData { ChangeToValue = "custom 4" }))
                .Execute(new ReproducibleTestCommandAsync(new ReproducibleTestData { ChangeToValue = "custom 5" }))
                ;

            Assert.NotNull(result);
            Assert.IsType<TypeSafeProxyContext>(result);
            Assert.Equal("custom 5", contextForTest.ContextVariable);
        }

        [Fact]
        public void Execute_WhenCreatingAGraphWithAProxyContext_ThenCommandIsExecutedOnTheMainObject()
        {
            var contextForTest = new ContextForTest();
            var proxyContext = new TypeSafeProxyContext(contextForTest);
            var graph = new GraphContext();

            graph.AddInput(proxyContext, "proxy-node-1");
            graph.ConfigureSetupAndInitialize();

            var executionDataResults = graph.Execute(new ReproducibleTestCommand(new ReproducibleTestData {ChangeToValue = "custom 1"}));

            Assert.Single(executionDataResults);
            Assert.NotNull(executionDataResults[0]);
            Assert.Null(executionDataResults[0].Exception);
            Assert.Equal(typeof(TypeSafeProxyContext), executionDataResults[0].SingleContextExecutionResultWithInfo.ContextType);
            Assert.Equal("custom 1", contextForTest.ContextVariable);
        }

        [Fact]
        public void Execute_WhenCreatingAGraphWithAProxyContext_ThenCommandsFlowNaturallyToNextContext()
        {
            var contextForTest = new ContextForTest();
            var proxyContext = new TypeSafeProxyContext(contextForTest);
            var contextForTest2 = new ContextForTest2();
            var graph = new GraphContext();

            graph.AddInput(proxyContext, "proxy-node-1");
            graph.AddNode(contextForTest2, "node-2");
            graph.ConnectNodes("proxy-node-1", "node-2");
            graph.ConfigureSetupAndInitialize();

            var executionDataResults = graph.Execute(new ActionForTestEventProducer("new-value"));

            Assert.Single(executionDataResults);
            Assert.NotNull(executionDataResults[0]);
            Assert.Null(executionDataResults[0].Exception);
            Assert.Equal(typeof(TypeSafeProxyContext), executionDataResults[0].SingleContextExecutionResultWithInfo.ContextType);
            Assert.Equal("new-value", contextForTest2.ContextVariable);
        }
    }
}
