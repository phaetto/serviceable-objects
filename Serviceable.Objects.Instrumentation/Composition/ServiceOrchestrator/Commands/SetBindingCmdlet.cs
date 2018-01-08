namespace Serviceable.Objects.Instrumentation.Composition.ServiceOrchestrator.Commands
{
    using System.Management.Automation;
    using Powershell;
    using Remote.Composition.ServiceOrchestrator.Commands;
    using Remote.Composition.ServiceOrchestrator.Commands.Data;

    [Cmdlet(VerbsCommon.Set, "Binding")]
    public sealed class SetBindingCmdlet : InstrumentationCommandCmdlet<SetBinding, BindingData>
    {
    }
}