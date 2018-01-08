namespace Serviceable.Objects.Remote.Composition.ServiceOrchestrator.Commands
{
    using System.Collections.Generic;
    using Objects.Composition.ServiceOrchestrator;

    public sealed class GetExternalBinding : RemotableCommandWithData<string, List<ExternalBinding>, ServiceOrchestratorContext>
    {
        public GetExternalBinding(string data) : base(data)
        {
        }

        public override List<ExternalBinding> Execute(ServiceOrchestratorContext context)
        {
            return context.ExternalBindingsPerService[Data];
        }
    }
}