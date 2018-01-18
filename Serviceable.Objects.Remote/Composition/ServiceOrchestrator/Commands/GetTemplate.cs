namespace Serviceable.Objects.Remote.Composition.ServiceOrchestrator.Commands
{
    public sealed class GetTemplate : RemotableCommandWithData<string, string, ServiceOrchestratorContext>
    {
        public GetTemplate(string data) : base(data)
        {
        }

        public override string Execute(ServiceOrchestratorContext context)
        {
            return context.GraphTemplatesDictionary[Data];
        }
    }
}