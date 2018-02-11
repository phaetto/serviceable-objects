namespace Serviceable.Objects.Instrumentation.Powershell
{
    using System.Management.Automation;
    using Newtonsoft.Json;
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
        public object Data { get; set; }

        public override TCommand GenerateCommand()
        {
            // For easier usage of untyped Object powershell structures we need to transform the input to json and then back to denormalize the types
            var dataAsJson = JsonConvert.SerializeObject(Data);

            return (TCommand)Types.CreateObjectWithParameters(typeof(TCommand), JsonConvert.DeserializeObject(dataAsJson, typeof(TDataType)));
        }
    }
}
