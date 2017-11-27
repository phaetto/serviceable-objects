namespace Serviceable.Objects.Instrumentation.CommonParameters
{
    using System.Management.Automation;

    public sealed class CommonInstrumentationParameters
    {
        public const string ContainerParameterSet = "Container";
        public const string ServiceParameterSet = "Service";
        public const string NodeParameterSet = "Node";
        public const string CustomPipeParameterSet = "Custom Pipe";
        
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, HelpMessage = "The container that you want to connect to", ParameterSetName = ContainerParameterSet)]
        public string ServiceContainerName { get; set; }

        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, HelpMessage = "The service that you want to connect to", ParameterSetName = ServiceParameterSet)]
        [Parameter(Mandatory = true, HelpMessage = "The service that you want to connect to", ParameterSetName = NodeParameterSet)]
        public string ServiceName { get; set; }

        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, HelpMessage = "The custom pipe that will transfer the command", ParameterSetName = CustomPipeParameterSet)]
        public string PipeName { get; set; }

        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, HelpMessage = "The node id that you want to connect to", ParameterSetName = NodeParameterSet)]
        [Parameter(Mandatory = true, HelpMessage = "The node id that you want to connect to", ParameterSetName = CustomPipeParameterSet)]
        public string NodeId { get; set; }

        [ValidateRange(100, int.MaxValue)]
        [Parameter(Mandatory = false, HelpMessage = "The timeout that the connection will fail")]
        public int TimeoutInMilliseconds { get; set; } = 1000;
    }
}