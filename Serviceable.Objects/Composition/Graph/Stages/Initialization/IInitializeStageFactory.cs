namespace Serviceable.Objects.Composition.Graph.Stages.Initialization
{
    public interface IInitializeStageFactory
    {
        object GenerateInitializationCommand();
        object GenerateDeinitializationCommand();
    }
}
