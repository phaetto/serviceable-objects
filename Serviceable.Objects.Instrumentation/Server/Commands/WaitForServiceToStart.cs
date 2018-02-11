namespace Serviceable.Objects.Instrumentation.Server.Commands
{
    using System.Threading.Tasks;
    using Remote;
    using Remote.Composition.Service;

    public class WaitForServiceToStart : ReproducibleCommand<InstrumentationServerContext, Task<InstrumentationServerContext>>
    {
        public override async Task<InstrumentationServerContext> Execute(InstrumentationServerContext context)
        {
            var serviceContext = context.GraphContext.Container.Resolve<ServiceContext>();
            await serviceContext.Execute(new Remote.Composition.Service.Commands.WaitForServiceToStart());

            return context;
        }
    }
}