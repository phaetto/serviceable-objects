namespace Serviceable.Objects.Remote.Composition.Service.Commands
{
    using Host;
    using Objects.Composition.Service;

    public class CloseService : ReproducibleCommand<IService, IService>
    {
        public override IService Execute(IService context)
        {
            var applicationHost = context.ServiceContainer.Resolve<ApplicationHost>();
            applicationHost.CancellationTokenSource.Cancel();

            return context;
        }
    }
}