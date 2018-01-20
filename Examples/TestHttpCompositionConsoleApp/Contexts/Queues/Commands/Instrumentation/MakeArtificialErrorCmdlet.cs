namespace TestHttpCompositionConsoleApp.Contexts.Queues.Commands.Instrumentation
{
    using System.Management.Automation;
    using Serviceable.Objects.Instrumentation.Powershell;

    [Cmdlet("Make", "ArtificialError")]
    public sealed class MakeArtificialErrorCmdlet : InstrumentationCommandCmdlet<MakeArtificialError>
    {
    }
}
