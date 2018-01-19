namespace Serviceable.Objects.Composition.Graph.Stages.Setup
{
    public interface ISetupStageFactory
    {
        object GenerateSetupCommand(GraphContext graphContext, GraphNodeContext graphNodeContext);
        object GenerateDismantleCommand(GraphContext graphContext, GraphNodeContext graphNodeContext);
    }
}