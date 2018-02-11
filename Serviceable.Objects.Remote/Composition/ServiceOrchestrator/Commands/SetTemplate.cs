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
            var deserializedData = JsonConvert.SerializeObject(Data);
            if (!string.IsNullOrWhiteSpace(Data.TemplateName))
            {
                context.GraphTemplatesDictionary[Data.TemplateName] = deserializedData;
            }

            if (!string.IsNullOrWhiteSpace(Data.ServiceName))
            {
                context.GraphTemplatesDictionary[Data.ServiceName] = deserializedData;
            }

            return context;
        }
    }
}