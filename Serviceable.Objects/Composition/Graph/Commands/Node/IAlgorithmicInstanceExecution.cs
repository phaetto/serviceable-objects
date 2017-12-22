namespace Serviceable.Objects.Composition.Graph.Commands.Node
{
    using System.Collections.Generic;
    using NodeInstance.ExecutionData;

    public interface IAlgorithmicInstanceExecution
    {
        IList<GraphNodeInstanceContext> FindGraphNodeInstanceContextsToBeExecuted(
            List<GraphNodeInstanceContext> graphNodeInstanceContexts
        );

        IList<GraphNodeInstanceContext> ContinueExecutionGraphNodeInstanceContextsToBeExecuted(
            IList<ExecutionCommandResult> previousExecutionCommandResults,
            List<GraphNodeInstanceContext> graphNodeInstanceContexts
        );

        IList<ExecutionCommandResult> FilterExecutionResults(
            IList<ExecutionCommandResult> previousExecutionCommandResults,
            IList<ExecutionCommandResult> currentExecutionCommandResults
        );
    }
}
