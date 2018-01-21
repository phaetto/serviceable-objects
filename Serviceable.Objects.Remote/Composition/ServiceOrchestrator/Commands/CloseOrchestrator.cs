namespace Serviceable.Objects.Remote.Composition.ServiceOrchestrator.Commands
{
    using Objects.Composition.ServiceOrchestrator;
    using Service;

    public class CloseOrchestrator : ReproducibleCommand<IServiceOrchestrator, IServiceOrchestrator>
    {
        public override IServiceOrchestrator Execute(IServiceOrchestrator context)
        {
            var serviceContext = context.ServiceOrchestratorContainer.Resolve<ServiceContext>();
            serviceContext.Execute(new Service.Commands.CloseService());

            return context;
        }
    }
}