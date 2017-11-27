namespace Serviceable.Objects.Remote.Composition.ServiceContainer.Commands
{
    using Graph;
    using Service;
    using Service.Configuration;

    public class StartServiceWithTemplate : ICommand<ServiceContainerContext, ServiceContainerContext>
    {
        private readonly string graphTemplate;
        private readonly string templateName;
        private readonly string serviceName;

        public StartServiceWithTemplate(string graphTemplate, string templateName, string serviceName)
        {
            this.graphTemplate = graphTemplate;
            this.templateName = templateName;
            this.serviceName = serviceName;
        }

        public ServiceContainerContext Execute(ServiceContainerContext context)
        {
            var service = new ServiceContext(new ServiceContextConfiguration
            {
                TemplateName = templateName,
                ServiceName = serviceName,
                ContainerName = context.ContainerName,
            }, context.ServiceContainerContextContainer);

            service.GraphContext.FromJson(graphTemplate);
            return context;
        }
    }
}