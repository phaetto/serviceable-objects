namespace Serviceable.Objects.Remote.Tests
{
    using Serviceable.Objects.Remote.Proxying;
    using Serviceable.Objects.Remote.Tests.Classes;
    using Serviceable.Objects.Remote.Tests.Classes.Proxies;
    using Serviceable.Objects.Tests.Classes;
    using System.Threading.Tasks;
    using Xunit;

    public class ProxyTests
    {
        [Fact]
        public void ProxyContext_WhenAReproducibleExecutes_ThenItCorrectlyProxiesIt()
        {
            var contextForTest = new ContextForTest();
            var proxyContext = new ProxyContext(contextForTest);

            var result = proxyContext.Execute(new ReproducibleTestAction(new ReproducibleTestData { ChangeToValue = "custom" }));

            Assert.NotNull(result);
            Assert.IsType<ProxyContext>(result);
            Assert.Equal("custom", contextForTest.ContextVariable);
        }

        [Fact]
        public void ProxyContext_WhenARemotableExecutes_ThenItCorrectlyProxiesIt()
        {
            var contextForTest = new ContextForTest();
            var proxyContext = new ProxyContext(contextForTest);

            var result = proxyContext.Execute(new RemotableTestAction(new ReproducibleTestData { ChangeToValue = "custom" }));

            Assert.NotNull(result);
            Assert.IsType<ReproducibleTestData>(result);
            Assert.Equal("custom", contextForTest.ContextVariable);
        }

        [Fact]
        public async Task ProxyContext_WhenAReproducibleAsyncExecutes_ThenItCorrectlyProxiesIt()
        {
            var contextForTest = new ContextForTest();
            var proxyContext = new ProxyContext(contextForTest);

            var result = await proxyContext.Execute(new ReproducibleTestActionAsync(new ReproducibleTestData { ChangeToValue = "custom" }));

            Assert.NotNull(result);
            Assert.IsType<ProxyContext>(result);
            Assert.Equal("custom", contextForTest.ContextVariable);
        }

        [Fact]
        public async Task ProxyContext_WhenARemotableAsyncExecutes_ThenItCorrectlyProxiesIt()
        {
            var contextForTest = new ContextForTest();
            var proxyContext = new ProxyContext(contextForTest);

            var result = await proxyContext.Execute(new RemotableTestActionAsync(new ReproducibleTestData { ChangeToValue = "custom" }));

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
                .Execute(new ReproducibleTestAction(new ReproducibleTestData { ChangeToValue = "custom 1" }))
                .Execute(new ReproducibleTestActionAsync(new ReproducibleTestData { ChangeToValue = "custom 2" }))
                .Execute(new ReproducibleTestAction(new ReproducibleTestData { ChangeToValue = "custom 3" }))
                .Execute(new ReproducibleTestActionAsync(new ReproducibleTestData { ChangeToValue = "custom 4" }))
                .Execute(new ReproducibleTestActionAsync(new ReproducibleTestData { ChangeToValue = "custom 5" }))
                ;

            Assert.NotNull(result);
            Assert.IsType<ProxyContext>(result);
            Assert.Equal("custom 5", contextForTest.ContextVariable);
        }

        //[Fact]
        //public void ProxyContext_WhenADualProxying_ThenItCorrectlyProxiesIt()
        //{
        //    var contextForTest = new ContextForTest();
        //    var proxyContext = new ProxyContext(contextForTest);
        //    var proxyContext2 = new ProxyContext(proxyContext); // Maybe this not be allowed?

        //    var result = proxyContext2.Execute(new ReproducibleTestAction(new ReproducibleTestData { ChangeToValue = "custom" }));

        //    Assert.NotNull(result);
        //    Assert.IsType<ProxyContext>(result);
        //    Assert.Equal("custom", contextForTest.ContextVariable);
        //}
    }
}
