namespace Serviceable.Objects.Composition.Graph.Commands.NodeInstance
{
    using Stages.Setup;

    public sealed class DismantleNodeInstance : ICommand<GraphNodeInstanceContext, GraphNodeInstanceContext>
    {
        public GraphNodeInstanceContext Execute(GraphNodeInstanceContext context)
        {
            if (context.HostedContext is ISetupStageFactory graphSetup)
            {
                var command = graphSetup.GenerateDismantleCommand(context.GraphContext, context.GraphNodeContext);

                if (command != null)
                {
                    context.HostedContext.Execute((dynamic) command);
                }
            }

            return context;
        }
    }
}