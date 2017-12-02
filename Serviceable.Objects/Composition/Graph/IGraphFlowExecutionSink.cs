namespace Serviceable.Objects.Composition.Graph
{
    public interface IGraphFlowExecutionSink
    {
        dynamic CustomCommandExecute(GraphContext graphContext, string executingNodeId, dynamic commandApplied);
    }
}