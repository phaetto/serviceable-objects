﻿namespace Serviceable.Objects.Instrumentation.CommonParameters
{
    using System.Management.Automation;

    public sealed class CommonInstrumentationParameters
    {
        public const string OrchestratorParameterSet = "Orchestrator";
        public const string ServiceParameterSet = "Service";
        public const string NodeParameterSet = "Node";
        public const string OrchestratorNodeParameterSet = "OrchestratorNode";
        public const string CustomPipeParameterSet = "Custom Pipe";

        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, HelpMessage = "The orchestrator that you want to connect to, or the service within that group", ParameterSetName = OrchestratorParameterSet)]
        [Parameter(Mandatory = true, HelpMessage = "The orchestrator that you want to connect to, or the service within that group", ParameterSetName = ServiceParameterSet)]
        [Parameter(Mandatory = true, HelpMessage = "The orchestrator that you want to connect to, or the service within that group", ParameterSetName = NodeParameterSet)]
        [Parameter(Mandatory = true, HelpMessage = "The orchestrator that you want to connect to, or the service within that group", ParameterSetName = OrchestratorNodeParameterSet)]
        public string ServiceOrchestrator { get; set; }

        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, HelpMessage = "The service that you want to connect to", ParameterSetName = ServiceParameterSet)]
        [Parameter(Mandatory = true, HelpMessage = "The service that you want to connect to", ParameterSetName = NodeParameterSet)]
        public string ServiceName { get; set; }

        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, HelpMessage = "The context id that this command aims to instrument (execution point)", ParameterSetName = NodeParameterSet)]
        [Parameter(Mandatory = true, HelpMessage = "The context id that this command aims to instrument (execution point)", ParameterSetName = OrchestratorNodeParameterSet)]
        public string ContextId { get; set; }

        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, HelpMessage = "The custom pipe that will transfer the command", ParameterSetName = CustomPipeParameterSet)]
        public string PipeName { get; set; }

        [ValidateRange(100, int.MaxValue)]
        [Parameter(Mandatory = false, HelpMessage = "The timeout that the connection will fail")]
        public int TimeoutInMilliseconds { get; set; } = 1000;
    }
}