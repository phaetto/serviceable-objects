namespace Serviceable.Objects.Remote.Composition.ServiceOrchestrator.Commands
{
    using System.Collections.Generic;
    using Objects.Composition.ServiceOrchestrator;

    public sealed class GetInBinding : RemotableCommandWithData<string, List<InBinding>, IServiceOrchestrator>
    {
        public GetInBinding(string data) : base(data)
        {
        }

        public override List<InBinding> Execute(IServiceOrchestrator context)
        {
            return context.InBindingsPerService[Data];
        }
    }
}