﻿namespace TestHttpCompositionConsoleApp.Contexts.ConsoleLog
{
    using System;
    using System.Collections.Generic;
    using Serviceable.Objects;
    using Serviceable.Objects.Composition.Graph;

    public sealed class ConsoleLogContext: Context<ConsoleLogContext>, IPostGraphFlowPullControl
    {
        public void PullNodeExecutionInformation(GraphContext graphContext, string executingNodeId, dynamic parentContext,
            dynamic parentCommandApplied, Stack<EventResult> eventResults)
        {
            Console.WriteLine("\n*** Executed ***" +
                              $"\n\tNode '{executingNodeId}'," +
                              $"\n\tContext: '{((object)parentContext).GetType().FullName}'," +
                              $"\n\tCommand: '{((object)parentCommandApplied).GetType().FullName}'" +
                              "\n\n");
        }
    }
}
