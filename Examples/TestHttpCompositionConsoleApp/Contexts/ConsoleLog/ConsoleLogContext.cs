namespace TestHttpCompositionConsoleApp.Contexts.ConsoleLog
{
    using System;
    using Serviceable.Objects;
    using Serviceable.Objects.Composition.Graph;

    public sealed class ConsoleLogContext: Context<ConsoleLogContext>, IPostGraphFlowPullControl
    {
        public void GetAttachNodeCommandExecutionInformation(GraphContext graphContext, string executingNodeId, dynamic parentContext,
            dynamic parentCommandApplied)
        {
            Console.WriteLine("\n*** Executed ***" +
                              $"\n\tNode '{executingNodeId}'," +
                              $"\n\tContext: '{((object)parentContext).GetType().FullName}'," +
                              $"\n\tCommand: '{((object)parentCommandApplied).GetType().FullName}'" +
                              "\n\n");
        }
    }
}
