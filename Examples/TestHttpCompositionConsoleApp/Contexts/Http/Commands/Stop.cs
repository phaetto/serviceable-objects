namespace TestHttpCompositionConsoleApp.Contexts.Http.Commands
{
    using Serviceable.Objects;

    public sealed class Stop : ICommand<OwinHttpContext, OwinHttpContext>
    {
        public OwinHttpContext Execute(OwinHttpContext context)
        {
            context.CancellationTokenSource.Cancel();
            return context;
        }
    }
}
