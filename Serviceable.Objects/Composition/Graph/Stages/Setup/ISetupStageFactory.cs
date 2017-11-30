namespace Serviceable.Objects.Composition.Graph.Stages.Setup
{
    public interface ISetupStageFactory
    {
        dynamic GenerateSetupCommand(GraphContext graphContext, GraphNodeContext graphNodeContext);
    }
}