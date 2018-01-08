namespace Serviceable.Objects.Remote.Composition.ServiceOrchestrator.Commands
{
    using System.Collections.Generic;
    using Objects.Composition.ServiceOrchestrator;

    public sealed class GetInBinding : RemotableCommandWithData<string, List<InBinding>, ServiceOrchestratorContext>
    {
        public GetInBinding(string data) : base(data)
        {
        }

        public override List<InBinding> Execute(ServiceOrchestratorContext context)
        {
            return context.InBindingsPerService[Data];
        }
    }
}