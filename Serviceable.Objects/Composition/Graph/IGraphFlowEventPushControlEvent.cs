namespace Serviceable.Objects.Composition.Graph
{
    using Commands.NodeInstance.ExecutionData;

    public interface IGraphFlowEventPushControlEvent : IEvent
    {
        ExecutionCommandResult OverrideEventPropagationLogic(GraphContext graphContext, string nodeId, object hostedContext);
    }
}
