namespace Serviceable.Objects.Remote.Composition.ServiceOrchestrator.Commands
{
    using Data;
    using Newtonsoft.Json;
    using Objects.Composition.ServiceOrchestrator;

    public sealed class SetTemplate : ReproducibleCommandWithData<IServiceOrchestrator, IServiceOrchestrator, GraphAndDependencyInjectionData>
    {
        public SetTemplate(GraphAndDependencyInjectionData data) : base(data)
        {
        }

        public override IServiceOrchestrator Execute(IServiceOrchestrator context)
        {
            context.GraphTemplatesDictionary[Data.ServiceName] = JsonConvert.SerializeObject(Data);
            return context;
        }
    }
}