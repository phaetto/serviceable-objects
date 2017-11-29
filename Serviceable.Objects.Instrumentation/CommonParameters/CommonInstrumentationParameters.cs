namespace Serviceable.Objects.Instrumentation.CommonParameters
{
    using System.Management.Automation;

    public sealed class CommonInstrumentationParameters
    {
        public const string ServiceParameterSet = "Service";
        public const string NodeParameterSet = "Node";
        public const string CustomPipeParameterSet = "Custom Pipe";
        
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, HelpMessage = "The service that you want to connect to", ParameterSetName = ServiceParameterSet)]
        [Parameter(Mandatory = true, HelpMessage = "The service that you want to connect to", ParameterSetName = NodeParameterSet)]
        public string ServiceName { get; set; }

        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, HelpMessage = "The custom pipe that will transfer the command", ParameterSetName = CustomPipeParameterSet)]
        public string PipeName { get; set; }

        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, HelpMessage = "The node id that you want to connect to", ParameterSetName = NodeParameterSet)]
        public string NodeId { get; set; }

        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, HelpMessage = "The context id that this command aims to instrument at", ParameterSetName = NodeParameterSet)]
        public string ContextId { get; set; }

        [ValidateRange(100, int.MaxValue)]
        [Parameter(Mandatory = false, HelpMessage = "The timeout that the connection will fail")]
        public int TimeoutInMilliseconds { get; set; } = 1000;
    }
}