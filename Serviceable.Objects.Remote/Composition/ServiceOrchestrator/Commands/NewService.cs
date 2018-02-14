namespace Serviceable.Objects.Remote.Composition.ServiceOrchestrator.Commands
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Data;
    using Events;
    using Objects.Composition.ServiceOrchestrator;

    public class NewService : ReproducibleCommandWithData<IServiceOrchestrator, IServiceOrchestrator, NewServiceData>, IEventProducer
    {
        public List<IEvent> EventsProduced { get; } = new List<IEvent>();

        public NewService(NewServiceData data) : base(data)
        {
        }

        public override IServiceOrchestrator Execute(IServiceOrchestrator context)
        {
            var containerPreparationManagementService = new ContainerPreparationManagementService(context, Data.TemplateName, Data.ServiceName);
            var startInfo = containerPreparationManagementService.PrepareStartInfoForProcess();

            var serviceProcess = new Process
            {
                StartInfo = startInfo,
            };

            serviceProcess.Start();

            EventsProduced.Add(new ServiceStarted {ProcessId = serviceProcess.Id, ServiceName = Data.ServiceName});

            return context;
        }
    }
}