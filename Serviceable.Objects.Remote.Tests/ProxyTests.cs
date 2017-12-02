namespace Serviceable.Objects.Remote.Tests
{
    using Proxying;
    using Classes;
    using Classes.Proxies;
    using Objects.Tests.Classes;
    using System.Threading.Tasks;
    using Xunit;

    public class ProxyTests
    {
        [Fact]
        public void ProxyContext_WhenAReproducibleExecutes_ThenItCorrectlyProxiesIt()
        {
            var contextForTest = new ContextForTest();
            var proxyContext = new ProxyContext(contextForTest);

            var result = proxyContext.Execute(new ReproducibleTestCommand(new ReproducibleTestData { ChangeToValue = "custom" }));

            Assert.NotNull(result);
            Assert.IsType<ProxyContext>(result);
            Assert.Equal("custom", contextForTest.ContextVariable);
        }

        [Fact]
        public void ProxyContext_WhenARemotableExecutes_ThenItCorrectlyProxiesIt()
        {
            var contextForTest = new ContextForTest();
            var proxyContext = new ProxyContext(contextForTest);

            var result = proxyContext.Execute(new RemotableTestCommand(new ReproducibleTestData { ChangeToValue = "custom" }));

            Assert.NotNull(result);
            Assert.IsType<ReproducibleTestData>(result);
            Assert.Equal("custom", contextForTest.ContextVariable);
        }

        [Fact]
        public async Task ProxyContext_WhenAReproducibleAsyncExecutes_ThenItCorrectlyProxiesIt()
        {
            var contextForTest = new ContextForTest();
            var proxyContext = new ProxyContext(contextForTest);

            var result = await proxyContext.Execute(new ReproducibleTestCommandAsync(new ReproducibleTestData { ChangeToValue = "custom" }));

            Assert.NotNull(result);
            Assert.IsType<ProxyContext>(result);
            Assert.Equal("custom", contextForTest.ContextVariable);
        }

        [Fact]
        public async Task ProxyContext_WhenARemotableAsyncExecutes_ThenItCorrectlyProxiesIt()
        {
            var contextForTest = new ContextForTest();
            var proxyContext = new ProxyContext(contextForTest);

            var result = await proxyContext.Execute(new RemotableTestCommandAsync(new ReproducibleTestData { ChangeToValue = "custom" }));

            Assert.NotNull(result);
            Assert.IsType<ReproducibleTestData>(result);
            Assert.Equal("custom", contextForTest.ContextVariable);
        }

        [Fact]
        public async Task ProxyContext_WhenACombinationExecutes_ThenItCorrectlyProxiesIt()
        {
            var contextForTest = new ContextForTest();
            var proxyContext = new ProxyContext(contextForTest);

            var result = await proxyContext
                .Execute(new ReproducibleTestCommand(new ReproducibleTestData { ChangeToValue = "custom 1" }))
                .Execute(new ReproducibleTestCommandAsync(new ReproducibleTestData { ChangeToValue = "custom 2" }))
                .Execute(new ReproducibleTestCommand(new ReproducibleTestData { ChangeToValue = "custom 3" }))
                .Execute(new ReproducibleTestCommandAsync(new ReproducibleTestData { ChangeToValue = "custom 4" }))
                .Execute(new ReproducibleTestCommandAsync(new ReproducibleTestData { ChangeToValue = "custom 5" }))
                ;

            Assert.NotNull(result);
            Assert.IsType<ProxyContext>(result);
            Assert.Equal("custom 5", contextForTest.ContextVariable);
        }
    }
}
