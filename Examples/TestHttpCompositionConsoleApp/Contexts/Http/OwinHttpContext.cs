namespace TestHttpCompositionConsoleApp.Contexts.Http
{
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Commands;
    using Configuration;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using Newtonsoft.Json;
    using Serviceable.Objects.Composition.Graph.Events;
    using Serviceable.Objects.Composition.Graph.Stages.Initialization;
    using Serviceable.Objects.Remote.Composition.Configuration;
    using Serviceable.Objects.Remote.Serialization;

    public sealed class OwinHttpContext : ConfigurableContext<OwinHttpContextConfiguration, OwinHttpContext>, IInitializeStageFactory
    {
        internal IWebHost Host;

        public OwinHttpContext(OwinHttpContextConfiguration configuration) : base(configuration)
        {
        }

        public OwinHttpContext(Serviceable.Objects.Composition.Graph.Stages.Configuration.IConfigurationSource configurationSource) : base(configurationSource)
        {
        }

        public dynamic GenerateInitializeCommand()
        {
            return new Run();
        }

        internal void SetupRouter(IRouteBuilder routerBuilder)
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
