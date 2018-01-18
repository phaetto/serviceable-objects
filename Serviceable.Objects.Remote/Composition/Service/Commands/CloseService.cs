namespace Serviceable.Objects.Remote.Composition.Service.Commands
{
    using Host;

    public class CloseService : ReproducibleCommand<ServiceContext, ServiceContext>
    {
        public override ServiceContext Execute(ServiceContext context)
        {
            //TODO: shut down the graph gracefully

            var applicationHost = context.ServiceContainer.Resolve<ApplicationHost>();
            applicationHost.CancellationTokenSource.Cancel();

            return context;
        }
    }
}