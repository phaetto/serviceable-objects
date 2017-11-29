namespace Serviceable.Objects.Instrumentation
{
    using System.Management.Automation;
    using Remote;
    using Remote.Dependencies;

    public abstract class InstrumentationCommandCmdlet<TCommand> : InstrumentationCommandCmdletBase<TCommand>
        where TCommand : IReproducible, IReproducibleWithoutData
    {
    }

    public abstract class InstrumentationCommandCmdlet<TCommand, TDataType> : InstrumentationCommandCmdletBase<TCommand>
        where TCommand : IReproducible, IReproducibleWithData
    {
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, Position = 0, HelpMessage = "The data that you would like to send")]
        public TDataType Data { get; set; }

        public override TCommand GenerateCommand()
        {
            return (TCommand)Types.CreateObjectWithParameters(typeof(TCommand), Data);
        }
    }
}
