namespace TestHttpCompositionConsoleApp.Contexts.Http.Commands
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Serviceable.Objects;

    public sealed class Run : ICommand<OwinHttpContext, Task<OwinHttpContext>>
    {
        public Task<OwinHttpContext> Execute(OwinHttpContext context)
        {
            return Task.Run(() =>
            {
                context.Host.Run();
                return context;
            });
        }
    }
}
