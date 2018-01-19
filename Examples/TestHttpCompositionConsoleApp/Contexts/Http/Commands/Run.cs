namespace TestHttpCompositionConsoleApp.Contexts.Http.Commands
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Serviceable.Objects;

    public sealed class Run : ICommand<OwinHttpContext, Task<OwinHttpContext>>
    {
        public Task<OwinHttpContext> Execute(OwinHttpContext context)
        {
            context.CancellationTokenSource = new CancellationTokenSource();

            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables().Build();

            context.Host = new WebHostBuilder()
                .UseKestrel()
                .UseConfiguration(config)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureLogging(l => l.AddConsole())
                .ConfigureServices(s => s.AddRouting())
                .Configure(app =>
                {
                    app.UseRouter(context.SetupRouter);
                })
                .UseUrls($"http://{context.Configuration.Host}:{context.Configuration.Port}")
                .Build();

            return Task.Run(() =>
            {
                context.Host.Run();
                return context;
            });
        }
    }
}
