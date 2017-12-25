namespace Serviceable.Objects.Remote.Composition.ServiceOrchestrator.Commands
{
    using Data;
    using Newtonsoft.Json;

    public sealed class SetTemplate : ReproducibleCommandWithData<ServiceOrchestratorContext, ServiceOrchestratorContext, GraphAndDependencyInjectionData>
    {
        public SetTemplate(GraphAndDependencyInjectionData data) : base(data)
        {
        }

        public override ServiceOrchestratorContext Execute(ServiceOrchestratorContext context)
        {
            context.GraphTemplatesDictionary[Data.Name] = JsonConvert.SerializeObject(Data);
            return context;
        }
    }
}