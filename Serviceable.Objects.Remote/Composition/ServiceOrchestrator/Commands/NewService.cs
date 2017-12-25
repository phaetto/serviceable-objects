namespace Serviceable.Objects.Remote.Composition.ServiceOrchestrator.Commands
{
    using System;
    using Data;

    public class NewService : ReproducibleCommandWithData<ServiceOrchestratorContext, ServiceOrchestratorContext, NewServiceData>
    {
        public NewService(NewServiceData data) : base(data)
        {
        }

        public override ServiceOrchestratorContext Execute(ServiceOrchestratorContext context)
        {
            if (!context.GraphTemplatesDictionary.ContainsKey(Data.ServiceName))
            {
                throw new InvalidOperationException($"Service {Data.ServiceName} does not exists");
            }

            return context;
        }
    }
}