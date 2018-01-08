namespace Serviceable.Objects.Remote.Composition.ServiceOrchestrator.Commands
{
    using System.Linq;
    using Data;

    public sealed class SetBinding : ReproducibleCommandWithData<ServiceOrchestratorContext, ServiceOrchestratorContext, BindingData>
    {
        public SetBinding(BindingData data) : base(data)
        {
        }

        public override ServiceOrchestratorContext Execute(ServiceOrchestratorContext context)
        {
            context.InBindingsPerService[Data.ServiceName] = Data.InBindings.ToList();
            context.ExternalBindingsPerService[Data.ServiceName] = Data.ExternalBindings.ToList();
            return context;
        }
    }
}