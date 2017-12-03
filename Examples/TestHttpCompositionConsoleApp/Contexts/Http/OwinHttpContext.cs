namespace TestHttpCompositionConsoleApp.Contexts.Http
{
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Commands;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Serviceable.Objects;
    using Serviceable.Objects.Composition.Graph.Events;
    using Serviceable.Objects.Composition.Graph.Stages.Initialization;
    using Serviceable.Objects.Remote.Serialization;

    public sealed class OwinHttpContext : Context<OwinHttpContext>, IInitializeStageFactory
    {
        public readonly IWebHost Host;

        public OwinHttpContext()
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables().Build();

            Host = new WebHostBuilder()
                .UseKestrel()
                .UseConfiguration(config)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureLogging(l => l.AddConsole())
                .ConfigureServices(s => s.AddRouting())
                .Configure(app =>
                {
                    app.UseRouter(SetupRouter);
                })
                .Build();
        }

        public dynamic GenerateInitializeCommand()
        {
            return new Run();
        }

        private void SetupRouter(IRouteBuilder routerBuilder)
        {
            routerBuilder.MapPost("test", TestRequestHandler);
        }

        private async Task TestRequestHandler(HttpContext context)
        {
            string data;
            using (var streamReader = new StreamReader(context.Request.Body))
            {
                data = streamReader.ReadToEnd();
            }

            var commandSpecification = JsonConvert.DeserializeObject<CommandSpecification>(data);
            var commandSpecificationService = new CommandSpecificationService();
            var command = commandSpecificationService.CreateCommandFromSpecification(commandSpecification);
            
            var eventResults =
                PublishContextEvent(new GraphFlowEventPushControlApplyCommandInsteadOfEvent(command))
                .Where(x => x.ResultObject != null).ToList();

            if (eventResults.Count > 0)
            {
                var results = eventResults.Select(x => x.ResultObject);
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(results));
            }
            else
            {
                context.Response.StatusCode = 204;
            }
        }
    }
}
