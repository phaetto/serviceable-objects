namespace Serviceable.Objects.Composition.Graph
{
    public interface IGraphFlowExecutionSink
    {
        object CustomCommandExecute(GraphContext graphContext, string executingNodeId, object commandApplied);
    }
}