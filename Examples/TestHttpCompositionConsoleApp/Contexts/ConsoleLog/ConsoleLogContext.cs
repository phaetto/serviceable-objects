namespace TestHttpCompositionConsoleApp.Contexts.ConsoleLog
{
    using System;
    using Serviceable.Objects;
    using Serviceable.Objects.Composition.Graph;

    public sealed class ConsoleLogContext: Context<ConsoleLogContext>, IPostGraphFlowPullControl
    {
        public void GetAttachNodeCommandExecutionInformation(GraphContext graphContext, string executingNodeId, object parentContext,
            object parentCommandApplied)
        {
            Console.WriteLine("\n*** Executed ***" +
                              $"\n\tNode '{executingNodeId}'," +
                              $"\n\tContext: '{parentContext.GetType().FullName}'," +
                              $"\n\tCommand: '{parentCommandApplied.GetType().FullName}'" +
                              "\n\n");
        }
    }
}
