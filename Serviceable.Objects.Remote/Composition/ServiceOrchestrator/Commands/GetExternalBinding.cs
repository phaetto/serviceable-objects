namespace Serviceable.Objects.Remote.Composition.ServiceOrchestrator.Commands
{
    using System.Collections.Generic;
    using Objects.Composition.ServiceOrchestrator;

    public sealed class GetExternalBinding : RemotableCommandWithData<string, List<ExternalBinding>, IServiceOrchestrator>
    {
        public GetExternalBinding(string data) : base(data)
        {
        }

        public override List<ExternalBinding> Execute(IServiceOrchestrator context)
        {
            return context.ExternalBindingsPerService[Data];
        }
    }
}