namespace Serviceable.Objects.Remote.Composition.Service.Commands
{
    using System.Threading.Tasks;
    using Objects.Composition.Graph;
    using Objects.Composition.Service;

    public class WaitForServiceToStart : ReproducibleCommand<IService, Task<IService>>
    {
        public override async Task<IService> Execute(IService context)
        {
            while (context.GraphContext.RuntimeExecutionState != RuntimeExecutionState.Running 
                   && context.GraphContext.MaxNodeStatus <= GraphNodeStatus.Initialized)
            {
                await Task.Delay(500);
            }

            return context;
        }
    }
}