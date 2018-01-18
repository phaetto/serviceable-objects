namespace Serviceable.Objects.Instrumentation.Server.Commands
{
    using Remote;
    using Remote.Composition.Service;

    public class CloseService : ReproducibleCommand<InstrumentationServerContext, InstrumentationServerContext>
    {
        public override InstrumentationServerContext Execute(InstrumentationServerContext context)
        {
            var serviceContext = context.GraphContext.Container.Resolve<ServiceContext>();
            serviceContext.Execute(new Remote.Composition.Service.Commands.CloseService());

            return context;
        }
    }
}