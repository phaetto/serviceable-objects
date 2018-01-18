namespace Serviceable.Objects.Remote.Composition.ServiceOrchestrator.Commands
{
    using Service;

    public class CloseOrchestrator : ReproducibleCommand<ServiceOrchestratorContext, ServiceOrchestratorContext>
    {
        public override ServiceOrchestratorContext Execute(ServiceOrchestratorContext context)
        {
            var serviceContext = context.ServiceOrchestratorContainer.Resolve<ServiceContext>();
            serviceContext.Execute(new Service.Commands.CloseService());

            return context;
        }
    }
}