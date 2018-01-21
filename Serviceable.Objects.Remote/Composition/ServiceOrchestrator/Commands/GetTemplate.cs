namespace Serviceable.Objects.Remote.Composition.ServiceOrchestrator.Commands
{
    using Objects.Composition.ServiceOrchestrator;

    public sealed class GetTemplate : RemotableCommandWithData<string, string, IServiceOrchestrator>
    {
        public GetTemplate(string data) : base(data)
        {
        }

        public override string Execute(IServiceOrchestrator context)
        {
            return context.GraphTemplatesDictionary[Data];
        }
    }
}